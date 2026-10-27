using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ReverseEngineer {
	public class TTypeParced {
		static string[] str_aux={ "", "[][]", "[]&", "*&", "**", "[]", "&", "*" };
		public static string GetAux(string name, out int aux, out string auxstr) {
			auxstr = "";
			for (aux = 1; aux < str_aux.Length; aux++) {
				if (name.EndsWith(str_aux[aux]))
					break;
			}
			if (aux >= str_aux.Length)
				aux = 0;
			auxstr = str_aux[aux];
			return name.Substring(0, name.Length - str_aux[aux].Length);
		}
		public TTypeParced(Type type) {
			ClassType = type;
			Name = "";
			string? Namespace;
			try {
				Namespace = ClassType.Namespace;
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", "TTypeParced", ex));
				return;
			}
			try {
				if (string.IsNullOrEmpty(ClassType.Name))
					return;
				Name = GetAux(ClassType.Name, out int ux, out string uxStr);
				Aux = ux;
				if (string.IsNullOrEmpty(ClassType.FullName)) {
					Name = "";
					return;
				}
				FullName = ClassType.FullName.Substring(0, ClassType.FullName.Length - str_aux[Aux].Length);
				ClassTypeRef = DotNetTypeNameParser.Parse(FullName);
				if (ClassTypeRef == null) {
					Name = "";
					return;
				}
				if (string.IsNullOrEmpty(ClassTypeRef.Name))
					RefName = "";
				else
					RefName = ClassTypeRef.Name;//.Substring(0, ClassTypeRef.Name.Length - str_aux[Aux].Length);
				Type? btype=ClassType.BaseType;
				if (btype != null)
					BaseFullName = btype.FullName ?? "";
				int i=0;
				while (btype != null) {
					btype = btype.BaseType;
					i++;
				}
				LevelInheritance = i;
				string[] spl=RefName.Split("+");
				ClassNestingLevel = spl.Length - 1;
				foreach (var item in spl) {
					int pos = item.IndexOf("`");
					if (pos >= 0) {
						string[] tmp=item.Split('`');
						int.TryParse(tmp[1], out i);
						TemplateParametersCount = i;
						break;
					}
				}
				Template = "";
				Instantiated = "";
				i = 1;
				if (ClassTypeRef.GenericArgs != null) {
					Template += "<";
					Instantiated += "<";
					foreach (TypeRef item in ClassTypeRef.GenericArgs) {
						Template += "T" + i + ",";
						Instantiated += item.Name + ",";
					}
					Template = Template.TrimEnd(',');
					Template += ">";
					Instantiated = Instantiated.TrimEnd(',');
					Instantiated += ">";
				}
				NameInstantiated = Name.Split("`")[0] + Instantiated;
				RefNameInstantiated = Namespace + "." + NameInstantiated;
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", ClassType.Namespace + " " + ClassType.Name, ex));
			}
		}
		public int Aux;
		public string AuxStr => str_aux[Aux];
		public Type ClassType;
		public Interop.RationalRose.RoseClass? RS_Class { get; set; }
		public TypeRef ClassTypeRef;
		/// <summary>
		/// Уровень вложенности шаблона
		/// </summary>
		public int TemplateNestingLevel => ClassTypeRef.TemplateNestingLevel;
		/// <summary>
		/// Сколько базовых классов
		/// </summary>
		public int LevelInheritance { get; protected set; }
		/// <summary>
		/// Уровень вложенности класса
		/// </summary>
		public int ClassNestingLevel { get; protected set; }
		public string Name { get; protected set; }
		public string RefName { get; protected set; }
		public string Template { get; protected set; }
		public string Instantiated { get; protected set; }
		public string NameInstantiated { get; protected set; }
		public string RefNameInstantiated { get; protected set; }
		public string FullName { get; protected set; }
		public string BaseFullName { get; protected set; }
		/// <summary>
		/// количество параметров шаблона
		/// </summary>
		public int TemplateParametersCount { get; protected set; }
	}
}
