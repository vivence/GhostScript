using System.Collections.Generic;

namespace Ghost.Script
{
	public class Interpreter
	{
		/*
		 * 名字层级：
		 * 全局类，全局函数，字面量
		 * 文件名+本地类/本地函数
		 * 类+常量/变量/函数
		 * 
		 * 行为：
		 * 文件本地入口函数
		 * 类静态加载函数
		 * 类实例构造，析构
		 * 
		 * 类描述：
		 * 字段类型
		 * 
		 * 函数描述：
		 * 参数类型（多个）
		 * 返回值类型（多个，推导）
		 * 
		 * 关键字：
		 * 
		 * 
		 * 语法：
		 * 
		 * 
		 * 存储结构：
		 * 描述区（类、函数）
		 * 堆（类常量，类对象）
		 * 栈（堆内索引，内置类型数据）
		 * 
		 * 指令集：
		 * 
		 */

		// 数组还是字典呢？如果是数组，那不能动态加载字节码，但是效率最高。
		#region description
		internal Description_Class[] classMap_;
		internal Description_Function[] functionMap_;
		#endregion description


	}
}

