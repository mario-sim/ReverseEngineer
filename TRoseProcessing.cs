using Interop.RationalRose;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReverseEngineer {
	public static class TRoseProcessing {
		#region Preparation
		static void PreparationTypeRef(Type? type) {
			if (type == null)
				return;
			if (type.FullName == null)
				return;
			TTypeParced tmp=new TTypeParced(type);
			if (string.IsNullOrEmpty(tmp.Name))
				return;
			if (!set_Type.Insert(tmp))
				return;
			set_TypeByFullName.Insert(tmp);
			FRose.PB.Increment(1);
			FRose.PB.Refresh();
			PreparationTypeRef(type.BaseType);
			try {
				MemberInfo[] members= type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				foreach (MemberInfo thisMember in members) {
					if (thisMember.DeclaringType == thisMember.ReflectedType) {
						if (!thisMember.IsDefined(typeof(CompilerGeneratedAttribute))) {
							switch (thisMember.MemberType) {
								case MemberTypes.Method: {
									if (thisMember is MethodInfo method)
										PreparationMethod(method);
									break;
								} //член является методом.
								case MemberTypes.Constructor: {
									if (thisMember is ConstructorInfo constructor)
										PreparationConstructor(constructor);
									break;
								} //элемент является конструктором.
								case MemberTypes.Event: {
									break;
								} //элемент является событием.
								case MemberTypes.Field: {
									if (thisMember is FieldInfo fieldInfo)
										PreparationTypeRef(fieldInfo.FieldType);
									break;
								} //член является полем.
								case MemberTypes.Property: {
									if (thisMember is PropertyInfo propertyInfo) {
										PreparationTypeRef(propertyInfo.PropertyType);
										if (propertyInfo.SetMethod != null)
											PreparationMethod(propertyInfo.SetMethod);
										if (propertyInfo.GetMethod != null)
											PreparationMethod(propertyInfo.GetMethod);
									}
									break;
								} //член является свойством.
								case MemberTypes.TypeInfo: {
									break;
								} //член является типом.
								case MemberTypes.Custom: {
									break;
								} //член является пользовательским типом члена.
								case MemberTypes.NestedType: {
									break;
								} //член является вложенным типом.
							}
						}
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", type.Namespace + " " + type.Name, ex));
			}
		}
		static void PreparationMethod(MethodInfo method) {
			try {
				PreparationTypeRef(method.ReturnType);
				ParameterInfo[] Arguments = method.GetParameters();
				foreach (ParameterInfo item in Arguments) {
					if (!item.IsRetval) {
						PreparationTypeRef(item.ParameterType);
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", method.Name, ex));
			}

		}
		static void PreparationConstructor(ConstructorInfo constructor) {
			try {
				ParameterInfo[] Arguments =constructor.GetParameters();
				foreach (var item in Arguments) {
					if (!item.IsRetval) {
						PreparationTypeRef(item.ParameterType);
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", constructor.Name, ex));
			}
		}
		#endregion
		static RoseCategory GetRoseAppNamespace(string? Namespace) {
			if (string.IsNullOrEmpty(Namespace))
				return Model.RootCategory;
			if (set_Category.TryGetValue(Namespace, out RoseCategory? rs_category))
				return rs_category;
			rs_category = Model.RootCategory;
			if (!string.IsNullOrEmpty(Namespace)) {
				string[] fields=Namespace.Split(".");
				foreach (var field in fields) {
					short i = rs_category.Categories.FindFirst(field);
					if (i == 0)
						rs_category = rs_category.AddCategory(field);
					else
						rs_category = rs_category.Categories.GetAt(i);
				}
			}
			try {
				set_Category.Add(Namespace, rs_category);
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", Namespace, ex));
			}
			return rs_category;
		}
		static RoseClass? GetRoseAppClass(Type? type, out string aux) {
			aux = "";
			if (type != null) {
				TTypeParced? typeParced =set_TypeByFullName.GetByKey(type);
				if (typeParced != null) {
					if (typeParced.RS_Class == null) {
						ProcessingTypeByRoseApp(typeParced, out aux);
					}
					return typeParced.RS_Class;
				}
				else {
					TTypeParced tmp=new TTypeParced(type);
					if (!string.IsNullOrEmpty(tmp.Name))
						set_TypeLost.Insert(tmp);
				}
			}
			return null;
		}
		static void RoseAppClassBody(RoseClass rs_class, Type type) {
			try {
				MemberInfo[] members= type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				foreach (MemberInfo thisMember in members) {
					if (thisMember.DeclaringType == thisMember.ReflectedType) {
						if (!thisMember.IsDefined(typeof(CompilerGeneratedAttribute))) {
							switch (thisMember.MemberType) {
								case MemberTypes.Method: {
									if (thisMember is MethodInfo method)
										RoseAppMethod(method, rs_class);
									break;
								} //член является методом.
								case MemberTypes.Constructor: {
									if (thisMember is ConstructorInfo constructor)
										RoseAppConstructor(constructor, rs_class);
									break;
								} //элемент является конструктором.
								case MemberTypes.Event: {
									break;
								} //элемент является событием.
								case MemberTypes.Field: {
									if (thisMember is FieldInfo fieldInfo)
										RoseAppField(fieldInfo, rs_class);
									break;
								} //член является полем.
								case MemberTypes.Property: {
									if (thisMember is PropertyInfo propertyInfo)
										RoseAppProperty(propertyInfo, rs_class);
									break;
								} //член является свойством.
								case MemberTypes.TypeInfo: {
									break;
								} //член является типом.
								case MemberTypes.Custom: {
									break;
								} //член является пользовательским типом члена.
								case MemberTypes.NestedType: {
									break;
								} //член является вложенным типом.
							}
						}
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", type.Namespace + " " + type.Name, ex));
			}

		}
		static void RoseAppTemplateBody(RoseClass rs_class, TTypeParced typeParced) {
			try {
				MemberInfo[] members= typeParced.ClassType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				foreach (MemberInfo thisMember in members) {
					if (thisMember.DeclaringType == thisMember.ReflectedType) {
						if (!thisMember.IsDefined(typeof(CompilerGeneratedAttribute))) {
							switch (thisMember.MemberType) {
								case MemberTypes.Method: {
									if (thisMember is MethodInfo method)
										RoseAppTemplateMethod(typeParced, method, rs_class);
									break;
								} //член является методом.
								case MemberTypes.Constructor: {
									if (thisMember is ConstructorInfo constructor)
										RoseAppTemplateConstructor(typeParced, constructor, rs_class);
									break;
								} //элемент является конструктором.
								case MemberTypes.Event: {
									break;
								} //элемент является событием.
								case MemberTypes.Field: {
									if (thisMember is FieldInfo fieldInfo)
										RoseAppTemplateField(typeParced, fieldInfo, rs_class);
									break;
								} //член является полем.
								case MemberTypes.Property: {
									if (thisMember is PropertyInfo propertyInfo)
										RoseAppTemplateProperty(typeParced, propertyInfo, rs_class);
									break;
								} //член является свойством.
								case MemberTypes.TypeInfo: {
									break;
								} //член является типом.
								case MemberTypes.Custom: {
									break;
								} //член является пользовательским типом члена.
								case MemberTypes.NestedType: {
									break;
								} //член является вложенным типом.
							}
						}
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.ClassType.Namespace + " " + typeParced.ClassType.Name, ex));
			}
		}
		static void RoseAppMethod(MethodInfo method, RoseClass rs_class) {//, string postfix = "") {
			RoseClass? rs_return_type = GetRoseAppClass(method.ReturnType,out string aux);
			RoseOperation rs_operation;
			if (rs_return_type == null)
				rs_operation = rs_class.AddOperation(method.Name, method.ReturnType.FullName);
			else
				rs_operation = rs_class.AddOperation(method.Name, rs_return_type.GetQualifiedName() + aux);
			if (method.IsPrivate)
				rs_operation.ExportControl.Name = "PrivateAccess";
			else if (method.IsPublic)
				rs_operation.ExportControl.Name = "PublicAccess";
			else
				rs_operation.ExportControl.Name = "ProtectedAccess";
			if (method.IsVirtual)
				rs_operation.Virtual = true;
			if (method.IsAbstract)
				rs_operation.Abstract = true;
			try {
				ParameterInfo[] Arguments =method.GetParameters();
				foreach (var item in Arguments) {
					if (!item.IsRetval) {
						RoseClass? rs_parameter_type = GetRoseAppClass(item.ParameterType,out aux);
						RoseParameter rs_parameter;
						if (rs_parameter_type == null)
							rs_parameter = rs_operation.AddParameter(item.Name, item.ParameterType.FullName, item.DefaultValue?.ToString() ?? "", (short)item.Position);
						else
							rs_parameter = rs_operation.AddParameter(item.Name, rs_parameter_type.GetQualifiedName() + aux, item.DefaultValue?.ToString() ?? "", (short)item.Position);
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", method.Name, ex));
			}
		}
		static string GetTypeTemplate(List<TypeRef>? GenericArgs, Type type) {
			string s_type="";
			if (type.FullName != null)
				if (GenericArgs != null) {
					int tt=1;
					foreach (TypeRef arg in GenericArgs) {
						if (arg.Name == TTypeParced.GetAux(type.FullName, out int aux, out string auxstr)) {
							s_type = "<T" + tt + ">" + auxstr;
							break;
						}
					}
				}
			if (s_type == "") {
				RoseClass? rs_type = GetRoseAppClass(type,out string aux);
				if (rs_type == null)
					s_type = type.FullName ?? "empty";
				else
					s_type = rs_type.GetQualifiedName() + aux;
			}
			return s_type;
		}
		static void RoseAppTemplateMethod(TTypeParced typeParced, MethodInfo method, RoseClass rs_class, string postfix = "") {
			string s_type=GetTypeTemplate(typeParced.ClassTypeRef.GenericArgs,method.ReturnType);
			RoseOperation rs_operation = rs_class.AddOperation(method.Name + postfix, s_type);
			if (method.IsPrivate)
				rs_operation.ExportControl.Name = "PrivateAccess";
			else if (method.IsPublic)
				rs_operation.ExportControl.Name = "PublicAccess";
			else
				rs_operation.ExportControl.Name = "ProtectedAccess";
			if (method.IsVirtual)
				rs_operation.Virtual = true;
			if (method.IsAbstract)
				rs_operation.Abstract = true;
			try {
				ParameterInfo[] Arguments =method.GetParameters();
				foreach (var item in Arguments) {
					if (!item.IsRetval) {
						s_type = GetTypeTemplate(typeParced.ClassTypeRef.GenericArgs, item.ParameterType);
						RoseParameter rs_parameter = rs_operation.AddParameter(item.Name, s_type, item.DefaultValue?.ToString() ?? "", (short)item.Position);
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", method.Name, ex));
			}
		}
		static void RoseAppConstructor(ConstructorInfo constructor, RoseClass rs_class) {
			if (constructor.ReflectedType == null)
				return;
			RoseOperation? rs_operation = rs_class.AddOperation(constructor.ReflectedType.Name, "");
			if (constructor.IsPrivate)
				rs_operation.ExportControl.Name = "PrivateAccess";
			else if (constructor.IsPublic)
				rs_operation.ExportControl.Name = "PublicAccess";
			else
				rs_operation.ExportControl.Name = "ProtectedAccess";
			if (constructor.IsVirtual)
				rs_operation.Virtual = true;
			if (constructor.IsAbstract)
				rs_operation.Abstract = true;
			try {
				ParameterInfo[] Arguments =constructor.GetParameters();
				foreach (var item in Arguments) {
					if (!item.IsRetval) {
						RoseClass? rs_parameter_type = GetRoseAppClass(item.ParameterType,out string faux);
						RoseParameter rs_parameter;
						if (rs_parameter_type == null)
							rs_parameter = rs_operation.AddParameter(item.Name, item.ParameterType.FullName, item.DefaultValue?.ToString() ?? "", (short)item.Position);
						else
							rs_parameter = rs_operation.AddParameter(item.Name, rs_parameter_type.GetQualifiedName() + faux, item.DefaultValue?.ToString() ?? "", (short)item.Position);
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", constructor.Name, ex));
			}
		}
		static void RoseAppTemplateConstructor(TTypeParced typeParced, ConstructorInfo constructor, RoseClass rs_class) {
			if (constructor.ReflectedType == null)
				return;
			RoseOperation? rs_operation = rs_class.AddOperation(constructor.ReflectedType.Name, "");
			if (constructor.IsPrivate)
				rs_operation.ExportControl.Name = "PrivateAccess";
			else if (constructor.IsPublic)
				rs_operation.ExportControl.Name = "PublicAccess";
			else
				rs_operation.ExportControl.Name = "ProtectedAccess";
			if (constructor.IsVirtual)
				rs_operation.Virtual = true;
			if (constructor.IsAbstract)
				rs_operation.Abstract = true;
			try {
				ParameterInfo[] Arguments =constructor.GetParameters();
				foreach (var item in Arguments) {
					if (!item.IsRetval) {
						string s_type = GetTypeTemplate(typeParced.ClassTypeRef.GenericArgs,item.ParameterType);
						RoseParameter rs_parameter = rs_operation.AddParameter(item.Name, s_type, item.DefaultValue?.ToString() ?? "", (short)item.Position);
					}
				}
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", constructor.Name, ex));
			}
		}
		static void RoseAppField(FieldInfo fieldInfo, RoseClass rs_class) {
			RoseClass? rs_attribute_type = GetRoseAppClass(fieldInfo.FieldType,out string faux);
			RoseAttribute? rs_attribute;
			if (rs_attribute_type == null)
				rs_attribute = rs_class.AddAttribute(fieldInfo.Name, fieldInfo.FieldType.FullName, "");
			else
				rs_attribute = rs_class.AddAttribute(fieldInfo.Name, rs_attribute_type.GetQualifiedName() + faux, "");
			if (fieldInfo.IsPrivate)
				rs_attribute.ExportControl.Name = "PrivateAccess";
			else if (fieldInfo.IsPublic)
				rs_attribute.ExportControl.Name = "PublicAccess";
			else
				rs_attribute.ExportControl.Name = "ProtectedAccess";
			if (fieldInfo.IsStatic)
				rs_attribute.Static = true;
		}
		static void RoseAppTemplateField(TTypeParced typeParced, FieldInfo fieldInfo, RoseClass rs_class) {
			string s_type = GetTypeTemplate(typeParced.ClassTypeRef.GenericArgs,fieldInfo.FieldType);
			RoseAttribute rs_attribute = rs_class.AddAttribute(fieldInfo.Name, s_type, "");
			if (fieldInfo.IsPrivate)
				rs_attribute.ExportControl.Name = "PrivateAccess";
			else if (fieldInfo.IsPublic)
				rs_attribute.ExportControl.Name = "PublicAccess";
			else
				rs_attribute.ExportControl.Name = "ProtectedAccess";
			if (fieldInfo.IsStatic)
				rs_attribute.Static = true;
		}
		static void RoseAppProperty(PropertyInfo propertyInfo, RoseClass rs_class) {
			RoseClass? rs_attribute_type = GetRoseAppClass(propertyInfo.PropertyType,out string faux);
			RoseAttribute? rs_attribute;
			if (rs_attribute_type == null) {
				rs_attribute = rs_class.AddAttribute(propertyInfo.Name, propertyInfo.PropertyType.FullName ?? "lsugasljdgh", "");
			}
			else {
				rs_attribute = rs_class.AddAttribute(propertyInfo.Name, rs_attribute_type.GetQualifiedName() + faux, "");
			}
			rs_attribute.ExportControl.Name = "PrivateAccess";
			if (propertyInfo.SetMethod != null)
				RoseAppMethod(propertyInfo.SetMethod, rs_class);
			if (propertyInfo.GetMethod != null)
				RoseAppMethod(propertyInfo.GetMethod, rs_class);
		}
		static void RoseAppTemplateProperty(TTypeParced typeParced, PropertyInfo propertyInfo, RoseClass rs_class) {
			string s_type = GetTypeTemplate(typeParced.ClassTypeRef.GenericArgs,propertyInfo.PropertyType);
			RoseAttribute rs_attribute = rs_class.AddAttribute(propertyInfo.Name, s_type, "");
			rs_attribute.ExportControl.Name = "PrivateAccess";
			if (propertyInfo.SetMethod != null)
				RoseAppTemplateMethod(typeParced, propertyInfo.SetMethod, rs_class, propertyInfo.Name);
			if (propertyInfo.GetMethod != null)
				RoseAppTemplateMethod(typeParced, propertyInfo.GetMethod, rs_class, propertyInfo.Name);
		}
		public static RoseApplication RoseApp;
		static RoseModel Model;
		static TRoseProcessing() {
			RoseApp = new RoseApplication();
			RoseModel? m_model = RoseApp.CurrentModel;
			if (m_model == null)
				m_model = RoseApp.NewModel();
			Model = m_model;
		}

		static void RoseAppClass(TTypeParced typeParced) {
			RoseCategory rs_category=GetRoseAppNamespace(typeParced.ClassType.Namespace);
			if (set_Class.TryGetValue(typeParced.RefName, out RoseClass? rs)) {
				typeParced.RS_Class = rs;
				return;
			}
			var i=rs_category.Classes.FindFirst(typeParced.Name);
			if (i == 0)
				typeParced.RS_Class = rs_category.AddClass(typeParced.Name);
			else
				typeParced.RS_Class = rs_category.Classes.GetAt(i);
			try {
				set_Class.Add(typeParced.RefName, typeParced.RS_Class);
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.RefName, ex));
			}
			RoseClass? rs_class_base=GetRoseAppClass(typeParced.ClassType.BaseType, out string aux);
			if (rs_class_base != null)
				typeParced.RS_Class.AddInheritRel("", rs_class_base.GetQualifiedName() + aux);
			RoseAppClassBody(typeParced.RS_Class, typeParced.ClassType);
			typeParced.RS_Class.ClassKind.Name = "NormalClass";
		}
		static void RoseAppClassNesting(TTypeParced typeParced) {
			if (typeParced.RS_Class != null)
				return;
			if (set_Class.TryGetValue(typeParced.RefName, out RoseClass? rs)) {
				typeParced.RS_Class = rs;
				return;
			}
			string[] part=typeParced.FullName.Split("+");
			RoseClass? rs_parent=null;
			int k = 0;
			if (k < part.Length - 1) {
				if (!set_Class.TryGetValue(part[k], out rs_parent))
					return;
			}
			string ff=part[k];
			for (k = 1; k < part.Length - 1; k++) {
				ff += "+" + part[k];
				if (!set_Class.TryGetValue(ff, out rs_parent))
					return;
			}
			if (rs_parent != null) {
				RoseCategory rs_category=GetRoseAppNamespace(typeParced.ClassType.Namespace);
				var i=rs_category.Classes.FindFirst(typeParced.Name);
				if (i == 0)
					typeParced.RS_Class = rs_category.AddClass(typeParced.Name);
				else
					typeParced.RS_Class = rs_category.Classes.GetAt(i);
				try {
					set_Class.Add(typeParced.RefName, typeParced.RS_Class);
				}
				catch (Exception ex) {
					Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.RefName, ex));
				}
				RoseClass? rs_class_base=GetRoseAppClass(typeParced.ClassType.BaseType, out string aux);
				if (rs_class_base != null)
					typeParced.RS_Class.AddInheritRel("", rs_class_base.GetQualifiedName() + aux);
				typeParced.RS_Class.ClassKind.Name = "NormalClass";
				rs_parent.AddNestedClass(typeParced.RS_Class.GetQualifiedName());
				RoseAppClassBody(typeParced.RS_Class, typeParced.ClassType);
			}
		}
		static void RoseAppTemplate(TTypeParced typeParced) {
			if (set_Class.TryGetValue(typeParced.RefNameInstantiated, out RoseClass? rs_instantiated)) {
				typeParced.RS_Class = rs_instantiated;
				return;
			}
			RoseCategory rs_category=GetRoseAppNamespace(typeParced.ClassType.Namespace);
			if (!set_Class.TryGetValue(typeParced.RefName, out RoseClass? rs_template)) {
				string temlateClass=typeParced.Name.Split('`')[0]+typeParced.Template;
				var i=rs_category.Classes.FindFirst(temlateClass);
				if (i == 0)
					rs_template = rs_category.AddClass(temlateClass);
				else
					rs_template = rs_category.Classes.GetAt(i);
				try {
					set_Class.Add(typeParced.RefName, rs_template);
				}
				catch (Exception ex) {
					Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.RefName, ex));
				}
				RoseClass? rs_template_class_base=GetRoseAppClass(typeParced.ClassType.BaseType, out string faux);
				if (rs_template_class_base != null)
					rs_template.AddInheritRel("", rs_template_class_base.GetQualifiedName() + faux);
				rs_template.ClassKind.Name = "ParameterizedClass";
				for (short k = 0; k < typeParced.TemplateParametersCount; k++)
					rs_template.AddParameter("T" + (k + 1), "", "", k);
				RoseAppTemplateBody(rs_template, typeParced);
			}
			if (set_Class.TryGetValue(typeParced.RefNameInstantiated, out RoseClass? rs_ins)) {
				typeParced.RS_Class = rs_ins;
				return;
			}
			string instantiatedClass=typeParced.Name.Split("`")[0];
			if (typeParced.ClassTypeRef.GenericArgs != null) {
				instantiatedClass += "<";
				foreach (var item in typeParced.ClassTypeRef.GenericArgs) {
					if (set_Class.TryGetValue(item.Name, out RoseClass? rs_item))
						instantiatedClass += rs_item.GetQualifiedName() + ",";
					else
						instantiatedClass += item.Name + ",";
				}
				instantiatedClass = instantiatedClass.TrimEnd(',');
				instantiatedClass += ">";
			}
			var ii=rs_category.Classes.FindFirst(instantiatedClass);
			if (ii == 0)
				typeParced.RS_Class = rs_category.AddClass(instantiatedClass);
			else
				typeParced.RS_Class = rs_category.Classes.GetAt(ii);
			try {
				set_Class.Add(typeParced.RefNameInstantiated, typeParced.RS_Class);
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.RefNameInstantiated, ex));
			}
			RoseAppClassBody(typeParced.RS_Class, typeParced.ClassType);
			typeParced.RS_Class.AddInstantiateRel(rs_template.GetQualifiedName());
			typeParced.RS_Class.ClassKind.Name = "InstantiatedClass";
		}
		static void RoseAppTemplateNesting(TTypeParced typeParced) {
			if (typeParced.RS_Class != null)
				return;
			if (set_Class.TryGetValue(typeParced.RefNameInstantiated, out RoseClass? rs)) {
				typeParced.RS_Class = rs;
				return;
			}
			string[] part=typeParced.FullName.Split("+");
			RoseClass? rs_parent=null;
			int k = 0;
			if (k < part.Length - 1) {
				if (!set_Class.TryGetValue(part[k], out rs_parent))
					return;
			}
			string ff=part[k];
			for (k = 1; k < part.Length - 1; k++) {
				ff += "+" + part[k];
				if (!set_Class.TryGetValue(ff, out rs_parent))
					return;
			}
			if (rs_parent != null) {
				RoseCategory rs_category=GetRoseAppNamespace(typeParced.ClassType.Namespace);
				if (!set_Class.TryGetValue(typeParced.RefName, out RoseClass? rs_template)) {
					string temlateClass=typeParced.Name.Split('`')[0]+typeParced.Template;
					var i=rs_category.Classes.FindFirst(temlateClass);
					if (i == 0)
						rs_template = rs_category.AddClass(temlateClass);
					else
						rs_template = rs_category.Classes.GetAt(i);
					try {
						set_Class.Add(typeParced.RefName, rs_template);
					}
					catch (Exception ex) {
						Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.RefName, ex));
					}
					RoseClass? rs_template_class_base=GetRoseAppClass(typeParced.ClassType.BaseType, out string faux);
					if (rs_template_class_base != null)
						rs_template.AddInheritRel("", rs_template_class_base.GetQualifiedName() + faux);
					rs_template.ClassKind.Name = "ParameterizedClass";
					for (k = 0; k < typeParced.TemplateParametersCount; k++)
						rs_template.AddParameter("T" + (k + 1), "", "", (short)k);
					RoseAppTemplateBody(rs_template, typeParced);
				}
				if (set_Class.TryGetValue(typeParced.RefNameInstantiated, out RoseClass? r1s)) {
					typeParced.RS_Class = r1s;
					return;
				}
				string instantiatedClass=typeParced.Name.Split("`")[0];
				if (typeParced.ClassTypeRef.GenericArgs != null) {
					instantiatedClass += "<";
					foreach (var item in typeParced.ClassTypeRef.GenericArgs) {
						if (set_Class.TryGetValue(item.Name, out RoseClass? rs_item))
							instantiatedClass += rs_item.GetQualifiedName() + ",";
						else
							instantiatedClass += item.Name + ",";
					}
					instantiatedClass = instantiatedClass.TrimEnd(',');
					instantiatedClass += ">";
				}
				var ii=rs_category.Classes.FindFirst(instantiatedClass);
				if (ii == 0)
					typeParced.RS_Class = rs_category.AddClass(instantiatedClass);
				else
					typeParced.RS_Class = rs_category.Classes.GetAt(ii);
				try {
					set_Class.Add(typeParced.RefNameInstantiated, typeParced.RS_Class);
				}
				catch (Exception ex) {
					Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.RefNameInstantiated, ex));
				}
				RoseAppClassBody(typeParced.RS_Class, typeParced.ClassType);
				typeParced.RS_Class.AddInstantiateRel(rs_template.GetQualifiedName());
				typeParced.RS_Class.ClassKind.Name = "InstantiatedClass";
				var nest = rs_parent.AddNestedClass(typeParced.RS_Class.GetQualifiedName());
			}
		}
		public static Lib.RBT.TRBTree<TTypeParced> set_Type { get; } = new Lib.RBT.TRBTree<TTypeParced>(new TTypeParcedComparer());
		public static Lib.RBT.TRBTree<TTypeParced> set_TypeLost { get; } = new Lib.RBT.TRBTree<TTypeParced>(new TTypeParcedComparer());
		public static Lib.RBT.TRBTree<TTypeParced> set_TypeByFullName { get; } = new Lib.RBT.TRBTree<TTypeParced>(new TTypeParcedTypeComparer());
		static SortedList<string,RoseCategory> set_Category=new SortedList<string,RoseCategory>();
		static SortedList<string,RoseClass> set_Class=new SortedList<string,RoseClass>();
		static void ProcessingTypeByRoseApp(TTypeParced typeParced, out string aux) {
			if (typeParced.RS_Class != null) {
				aux = typeParced.AuxStr;
				return;
			}
			if (typeParced.ClassType == null) {
				aux = "";
				return;
			}
			if (typeParced.ClassType.FullName == null) {
				aux = "";
				return;
			}
			string? Namespace;
			try {
				Namespace = typeParced.ClassType.Namespace;
			}
			catch (Exception ex) {
				Lib.Logger.TLoggerContainer.NLogger?.Log(Lib.Logger.TLoggerContainer.Error("actor", "action", typeParced.ClassType.FullName, ex));
				aux = "";
				return;
			}
			if (set_Class.TryGetValue(typeParced.FullName, out RoseClass? rs)) {
				typeParced.RS_Class = rs;
				aux = typeParced.AuxStr;
				return;
			}
			FRose.PB.Increment(1);
			FRose.PB.Refresh();
			if (typeParced.TemplateParametersCount == 0) {
				if (typeParced.ClassNestingLevel == 0) {
					RoseAppClass(typeParced);
					if (typeParced.RS_Class != null)
						aux = typeParced.AuxStr;
					else
						aux = "";
				}
				else {
					RoseAppClassNesting(typeParced);
					if (typeParced.RS_Class != null)
						aux = typeParced.AuxStr;
					else
						aux = "";
				}
			}
			else {
				if (typeParced.ClassNestingLevel == 0) {
					RoseAppTemplate(typeParced);
					if (typeParced.RS_Class != null)
						aux = typeParced.AuxStr;
					else
						aux = "";
				}
				else {
					RoseAppTemplateNesting(typeParced);
					if (typeParced.RS_Class != null)
						aux = typeParced.AuxStr;
					else
						aux = "";
				}
			}
		}
		public static void Preparation(List<Type> types) {
			RoseApp.Visible = false;
			set_Type.Clear();
			set_TypeLost.Clear();
			set_TypeByFullName.Clear();
			set_Category.Clear();
			set_Class.Clear();
			foreach (Type type in types) {
				PreparationTypeRef(type);
			}
		}
		public static void Processing() {
			var array=set_Type.ToArrayAscending();
			foreach (TTypeParced item in array) {
				ProcessingTypeByRoseApp(item, out string aux);
			}
		}
	}
}
