# LispParser-BlockCipherParser
此仓库为本人在课程《编程语言》（深圳大学）中的第二次作业，为简单的Lisp解释器和分组加密算法解释器

--- 

## LispParser——一个简单的Lisp语言解释器
> Lisp 语言由John McCarthy 教授(1927-2011，1971 年图灵奖得主)发明于
1958 年，是现今还在使用的第二古老的高级语言(最古老的是Fortran，比Lisp
发明还早一年)。
#### 能做的事情
- 运算符: +、-、*、/
- 函数: eq、define、lambda、条件表达式cond
- 示例程序: 定义函数、递归、条件判断、lambda等测试代码
#### 特点
- 视所有东西皆为函数，包括一个整数也视为一个函数（其函数输出一个整数）
- 动态的绑定、解除绑定变量，lambda嵌套中可以前面已经定义的变量
#### 注意
- 不能检查语法、语义错误
- 遇到没有定义、没有意义的代码将会抛出异常，结束程序
--- 
## BlockCipherParser——一个简单的分组加密算法语言解释器
> 在密码学中，分组加密(block cipher)，又称分块加密或块密码，是一种对称
密钥算法。它将明文分成多个等长的模块(block)，使用确定的算法和对称密钥
对每组分别加密解密。分组加密是极其重要的加密协议组成，其中典型的如 DES
和 AES 作为美国政府核定的标准加密算法，应用领域从电子邮件加密到银行交
易转帐，非常广泛。(摘自维基百科)

> 该语言的结构大致如下:
> 1. 变量声明部分
> 2. BEGIN
> 3. 加密算法部分
> 4. END
#### 能做的事情
- 表达式运算
- 运算符: +、=、置换（P）、查找（S）
- 逻辑控制: 循环LOOP、切割
- 示例代码: 简单的测试代码
#### 注意
- 由于ddl时间紧迫，使用的是简单粗暴的表达式运算方法，因此不能很好的处理运算符优先级问题
- 容错性较低，不能很好的处理语法错误问题
---

## 开发平台及语言
- Visual Studio 2017
- C#

---

## License & Release
见<a href="https://github.com/UnknownArkish/LispParser-BlockCipherParser/blob/dev/LICENSE">LICENSE.md</a>

---
## 如何使用
- 使用Visual Studio2017（或更高版本）打开PL_HW_2.sln（VS解决方案文件），编译即可运行
