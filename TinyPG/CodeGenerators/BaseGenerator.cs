using System;
using System.Collections.Generic;
using System.Text;

namespace TinyPG.CodeGenerators
{
	public class BaseGenerator
	{
		protected string templateName;
		private string fileName;

		public BaseGenerator(string templateName)
		{
			this.templateName = templateName;
			FileName = templateName;

		}

		public virtual string FileName
		{
			get { return this.fileName; }
			set { this.fileName = value; }
		}
	}
}
