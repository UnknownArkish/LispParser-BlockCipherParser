﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlockCipher
{
    /// <summary>
    /// 处理S盒的Handler
    /// </summary>
    public class SBoxHandler : ExpressionHandler
    {
        public SBoxHandler(ExpressionParser expressionParser) : base(expressionParser)
        {
        }

        public override BitArray Handle(string expression)
        {
            int pIndex = GetPIndex(expression);
            BitArray op = GetOperator(expression);
            PSData permute = Parser.PermuteSBoxStorage.SBox[pIndex];

            BitArray result = PermuteCalculate(permute, op);
            return result;
        }


        private BitArray PermuteCalculate(PSData sBox, BitArray op)
        {
            // 将op转为十进制
            int opInt = Convert.ToInt32(BlockCipherUtil.BitArrayToString(op), 2);
            int resultInt = sBox.Data[opInt];
            // 将temp转为二进制字符串
            StringBuilder resultStr = new StringBuilder(Convert.ToString(resultInt, 2));
            for (int i = resultStr.Length; i < sBox.OutputLength; i++)
            {
                resultStr.Insert(0, "0");
            }

            BitArray result = BlockCipherUtil.StringToBitArray(resultStr.ToString());
            return result;
        }

        /// <summary>
        /// 获取置换盒的下标
        /// </summary>
        private int GetPIndex(string expresstion)
        {
            Regex reg = new Regex(@"\[\w+\]");
            Match match = reg.Match(expresstion);

            // 绑定后取出其中的数字部分
            string temp = BindingVariable(match.Value);
            reg = new Regex(@"\w+");
            match = reg.Match(temp);
            int result = int.Parse(match.Value);
            return result;
        }

        /// <summary>
        /// 获取操作数
        /// </summary>
        private BitArray GetOperator(string expression)
        {
            Regex reg = new Regex(@"\(\S+\)");
            Match match = reg.Match(expression);
            string resultExpression = match.Value.Substring(1);
            resultExpression = resultExpression.Substring(0, resultExpression.Length - 1);
            return ExpressionParser.ParseExpression(resultExpression);
        }
    }
}