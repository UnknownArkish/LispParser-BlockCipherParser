﻿using System;
using System.Collections.Generic;
using System.Linq;



public class Signal
{
    /// <summary>
    /// 参数编号
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 标识名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 绑定的参数数值
    /// </summary>
    public string BindingValue { get; set; }
}
public class Template
{
    public int Index { get; set; }
    /// <summary>
    /// 模板的名字
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 绑定的数值
    /// </summary>
    public string BindingValue { get; set; }
}

/// <summary>
/// 所有函数原子的基类
/// </summary>
public abstract class BaseAtom
{
    public LispParser Parser { get; private set; }

    /// <summary>
    /// 接收参数个数
    /// </summary>
    public int SignalNum { get { return m_SignalDict.Count; } }


    private string m_SignalStr;
    /// <summary>
    /// Signal字典
    /// </summary>
    private Dictionary<string, Signal> m_SignalDict;
    /// <summary>
    /// 返回此函数原子的所有Signals
    /// </summary>
    public Signal[] Signals { get { return m_SignalDict.Values.ToArray(); } }


    private string m_TemplateStr;
    /// <summary>
    /// 操作数Template
    /// </summary>
    protected Template m_Operator;
    /// <summary>
    /// Template字典
    /// </summary>
    private Dictionary<int, Template> m_TemplateDict;
    /// <summary>
    /// 返回此函数原子的所有Template
    /// </summary>
    public Template[] Templates { get { return m_TemplateDict.Values.ToArray(); } }


    /// <param name="signalsStr">形如(x y)</param>
    /// <param name="templateStr">形如(??? x y)</param>
    public BaseAtom(LispParser parser, string signalsStr, string templateStr)
    {
        Parser = parser;
        m_SignalDict = new Dictionary<string, Signal>();
        m_TemplateDict = new Dictionary<int, Template>();

        LoadSignalStr(signalsStr);
        LoadTemplateStr(templateStr);
    }
    private void LoadSignalStr(string signalStr)
    {
        m_SignalStr = signalStr;
        m_SignalDict.Clear();

        signalStr = LispUtil.RemoveBracket(signalStr);
        if (string.IsNullOrEmpty(signalStr)) return;
        string[] signalStrs = signalStr.Split(' ');
        //SignalNum = signalStrs.Length;
        for (int i = 0; i < signalStrs.Length; i++)
        {
            m_SignalDict[signalStrs[i]] = new Signal()
            {
                Index = i,
                Name = signalStrs[i],
                BindingValue = null
            };
        }
    }
    private void LoadTemplateStr(string templateStr)
    {
        m_TemplateStr = templateStr;
        m_TemplateDict.Clear();

        templateStr = LispUtil.RemoveBracket(templateStr);
        if (string.IsNullOrEmpty(templateStr)) return;
        string[] templateStrs = LispUtil.SplitInAtomAll(templateStr);

        for (int i = 0; i < templateStrs.Length; i++)
        {
            Template template = new Template
            {
                Index = i - 1,
                Name = templateStrs[i],
                BindingValue = templateStrs[i]
            };
            if (i == 0)
            {
                m_Operator = template;
            }
            else
            {
                m_TemplateDict[i - 1] = template;
            }
        }
    }


    public virtual BaseAtom Run(string list)
    {
        // @TODO: 解析list中的参数
        string[] args = GetArgs(list);
        // 先对args进行绑定，因为可能args中有些东西已经被定义过了
        BindingArgs(args);
        // @TODO: 对SignalsDict进行Signal的BindingValue进行绑定
        BindingSignalValue(args);
        // @TODO：向RuntimeAtomStack注册此Atom
        Parser.RuntimeAtomStack.RegisterSignals(this);
        // @TODO：对所有Template<替换>以后进行<ParserList>
        BindingTemplateValue();
        Parser.RuntimeAtomStack.RegisterTemplate(this);
        Parser.RuntimeAtomStack.RegisterAtom(this);

        // 将所有的templateResult交给具体自类处理
        BaseAtom result = this.Handle(m_Operator);
        // @TODO：向RuntimeAtomStack取消注册此Atom
        Parser.RuntimeAtomStack.Unregister(this);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    protected abstract BaseAtom Handle(Template operand);
    public abstract object GetResult();

    public void BindingArgs(string[] args)
    {
        for( int i = 0; i < args.Length; i++)
        {
            if( LispUtil.IsAtom(args[i]))
            {
                // @TODO：先从RuntimeStack中取出数值
                string temp = Parser.RuntimeAtomStack.GetSignalValue(args[i]);
                // @TODO：如果为null，再从AtomStorage中取出数值
                if (temp != null) args[i] = temp;
            }
            args[i] = Parser.ParseAndGetResult(args[i]) as string;
        }
    }

    /// <summary>
    /// 绑定Signal
    /// </summary>
    private void BindingSignalValue(string[] args)
    {
        foreach (Signal signal in Signals)
        {
            signal.BindingValue = args[signal.Index];
        }
    }
    /// <summary>
    /// 绑定TemplateValue，如果是非原子不会进行绑定
    /// </summary>
    private void BindingTemplateValue()
    {
        for (int i = 0; i < Templates.Length; i++)
        {
            Template template = Templates[i];
            string toBindingValue = template.Name;

            // 如果非原子，则从库中取出
            if (LispUtil.IsAtom(toBindingValue))
            {
                // 先从Atom调用栈中除去数值
                string temp = BindFromRuntimeStack(toBindingValue);
                if (temp != null)
                {
                    toBindingValue = temp;
                }
            }
            template.BindingValue = toBindingValue;
        }
    }

    // 从动态运行栈中绑定
    protected string BindFromRuntimeStack(string toBind)
    {
        return Parser.RuntimeAtomStack.GetSignalValue(toBind);
    }

    /// <summary>
    /// 解析指定的已绑定Value的Template
    /// </summary>
    protected BaseAtom ParseTemplate(Template template)
    {
        BaseAtom result = null;
        if (LispUtil.IsAtom(template.BindingValue))
        {
            result = Parser.AtomStorage[template.BindingValue];
        }
        else
        {
            result = Parser.ParseAndGetAtom(template.BindingValue);
        }
        return result;
    }
    /// <summary>
    /// 解析所有的已绑定的Template
    /// </summary>
    /// <returns></returns>
    protected BaseAtom[] ParseTemplateAll()
    {
        BaseAtom[] results = new BaseAtom[Templates.Length];
        for (int i = 0; i < Templates.Length; i++)
        {
            results[i] = ParseTemplate(Templates[i]);
        }
        return results;
    }


    protected string[] GetArgs(string list)
    {
        string[] args = LispUtil.SplitInAtom(list, SignalNum + 1);
        string[] result = new string[SignalNum];
        for (int i = 0; i < SignalNum; i++)
        {
            result[i] = args[i + 1];
        }
        return result;
    }


}
