using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

/*
 * 编译规则：
 * 文件头（校验值，自定义）
 * 启动文件：
 * 	字节码文件列表（元素为字节码文件ID、版本、校验值）
 * 	全局符号列表（用于支持增量编译，元素为符号全名（可分解出字节码文件用于即时加载），可空缺，分类：类、函数、常量）
 * 	外部接口列表（元素为接口名，允许一级名字搜索域）
 * 	导出接口映射表（[接口名,符号索引]）
 * 字节码文件：文件头自定义（ID、版本），类描述，函数描述
 * 文件相对路径--[生成]-->文件ID（唯一）--[作为文件名]-->字节码文件
 */ 

/*
 * 特殊语法：
 * 标识符以下划线结尾则为本地私有，类中则为类私有，文件中则为文件私有
 * 必须使用this.才能访问类实例中的标识符
 * 必须使用class.（或者类名.）才能访问类静态标识符
 * Main(string[] args) end，console入口函数
 * 
 */

/*
#define LOG_LINE // 打印当前扫描行
#define LOG_NOTE // 打印当前扫描出的注释
#define LOG_LEX  // 打印当前扫描出的词
*/

namespace Ghost.Script
{
	public class MainClass
	{
		internal static void Log_LexException(LexException e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(String.Format("LexException: {0}, {1}\n at [{2},{3}]: ", e.errorCode.ToString(), e.content, e.row, e.col));
			if (null != e.line && 0 < e.col && e.line.Length >= e.col)
			{
				Console.ForegroundColor = ConsoleColor.Black;
				Console.Write(e.line.Substring(0, e.col-1));
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write(e.line[e.col-1]);
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine(e.line.Substring(e.col));
			}
		}

		internal static void Log_SyntaxException(SyntaxException e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(String.Format("SyntaxException: {0}, {1}\n at [{2},{3}]: ", e.errorCode.ToString(), e.content, e.row, e.col));
			if (null != e.line && 0 < e.col && e.line.Length >= e.col)
			{
				Console.ForegroundColor = ConsoleColor.Black;
				Console.Write(e.line.Substring(0, e.col-1));
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write(e.line[e.col-1]);
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine(e.line.Substring(e.col));
			}
		}

		public static void Main (string[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				FileStream fs = null;
				StreamReader reader = null;
				try
				{
					fs = new FileStream(
						args[i], 
						FileMode.Open, 
						FileAccess.Read, 
						FileShare.Read);
					reader = new StreamReader(fs, Encoding.UTF8);
					Syntax.ParseStream(reader, args[i]);
				}
				catch (LexException e)
				{
					Log_LexException(e);
				}
				catch (SyntaxException e)
				{
					Log_SyntaxException(e);
				}
				catch (Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(String.Format("Exception: \n{0}", e));
				}
				finally
				{
					if (null != reader)
					{
						reader.Close();
						reader = null;
					}
					if (null != fs)
					{
						fs.Close();
						fs = null;
					}
				}
			}
		}
	}
}