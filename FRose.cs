using Interop.RationalRose;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ReverseEngineer {
	public partial class FRose : Form {
		public static ProgressBar PB;
		public FRose() {
			InitializeComponent();
			PB = progressBar1;
		}
		#region ByRoseApp
		#endregion
		private void button1_Click(object sender, EventArgs e) {
			using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
				// Настройка диалогового окна
				openFileDialog.InitialDirectory = "D:\\work\\CSharp";
				openFileDialog.Multiselect = true;
				openFileDialog.Filter = "DLL files (*.dll)|*.dll";
				openFileDialog.FilterIndex = 1; // По умолчанию выбран фильтр "Text files"
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Title = "Выберите файлы";

				// Отображение диалога и проверка результата
				if (openFileDialog.ShowDialog() == DialogResult.OK) {
					try {
						List<Type> types=new List<Type>();
						string[] files = openFileDialog.FileNames;
						if (files.Length > 0) {
							foreach (string file in files) {
								Assembly? sourceAssembly = Assembly.LoadFrom(file);
								if (sourceAssembly != null)
									types.AddRange(sourceAssembly.GetTypes());
							}
							PB.Minimum = 0;
							PB.Value = 0;
							PB.Maximum = types.Count;
							TRoseProcessing.Preparation(types);
							PB.Value = 0;
							PB.Maximum = TRoseProcessing.set_Type.Count;
							TRoseProcessing.Processing();
							using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
								// Настройка диалога
								saveFileDialog.Title = "Сохранить файл";
								saveFileDialog.Filter = "Текстовые файлы (*.mdl)|*.mdl|Все файлы (*.*)|*.*";
								saveFileDialog.FilterIndex = 1;
								saveFileDialog.DefaultExt = "mdl";
								saveFileDialog.AddExtension = true;
								saveFileDialog.OverwritePrompt = true;
								saveFileDialog.RestoreDirectory = true;
								saveFileDialog.InitialDirectory = openFileDialog.InitialDirectory;
								saveFileDialog.FileName = Path.GetFileNameWithoutExtension(files[0]) + ".mdl";
								if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
									try {
										TRoseProcessing.RoseApp.SaveAs(saveFileDialog.FileName, false);
										/*string res =
																		"\tFullName" +
																		"\tBaseFullName" +
																		"\tTypeFullName" +
																		"\tName" +
																		"\tRefName" +
																		"\tRefNameInstantiated" +
																		"\tAux" +
																		"\tLI" +
																		"\tTNL" +
																		"\tTPC" +
																		"\tCNL" +
																		"\tNamespace" +
																		"\tBaseType" +
																		"\tAssembly" +
																		"\tAssemblyQualifiedName" +
																		"\tAttributes" +
																		"\tContainsGenericParameters" +
																		"\tCustomAttributes" +
																		"\tDeclaringMethod" +
																		"\tDeclaringType" +
																		"\tGUID" +
																		"\tGenericParameterAttributes" +
																		"\tGenericParameterPosition" +
																		"\tGenericTypeArguments" +
																		"\tHasElementType" +
																		"\tIsAbstract" +
																		"\tIsAnsiClass" +
																		"\tIsArray" +
																		"\tIsAutoClass" +
																		"\tIsAutoLayout" +
																		"\tIsByRef" +
																		"\tIsByRefLike" +
																		"\tIsCOMObject" +
																		"\tIsClass" +
																		"\tIsCollectible" +
																		"\tIsConstructedGenericType" +
																		"\tIsContextful" +
																		"\tIsEnum" +
																		"\tIsExplicitLayout" +
																		"\tIsFunctionPointer" +
																		"\tIsGenericMethodParameter" +
																		"\tIsGenericParameter" +
																		"\tIsGenericType" +
																		"\tIsGenericTypeDefinition" +
																		"\tIsGenericTypeParameter" +
																		"\tIsImport" +
																		"\tIsInterface" +
																		"\tIsLayoutSequential" +
																		"\tIsMarshalByRef" +
																		"\tIsNested" +
																		"\tIsNestedAssembly" +
																		"\tIsNestedFamANDAssem" +
																		"\tIsNestedFamORAssem" +
																		"\tIsNestedFamily" +
																		"\tIsNestedPrivate" +
																		"\tIsNestedPublic" +
																		"\tIsNotPublic" +
																		"\tIsPointer" +
																		"\tIsPrimitive" +
																		"\tIsPublic" +
																		"\tIsSZArray" +
																		"\tIsSealed" +
																		"\tIsSecurityCritical" +
																		"\tIsSecuritySafeCritical" +
																		"\tIsSecurityTransparent" +
																		"\tIsSignatureType" +
																		"\tIsSpecialName" +
																		"\tIsTypeDefinition" +
																		"\tIsUnicodeClass" +
																		"\tIsUnmanagedFunctionPointer" +
																		"\tIsValueType" +
																		"\tIsVariableBoundArray" +
																		"\tIsVisible" +
																		"\tMemberType" +
																		"\tMetadataToken" +
																		"\tModule" +
																		"\tReflectedType" +
																		"\tStructLayoutAttribute" +
																		"\tTypeHandle" +
																		"\tTypeInitializer" +
																		"\tUnderlyingSystemType" +
																											"\r\n";
										var array=TRoseProcessing.set_Type.ToArrayAscending();
										foreach (TTypeParced item in array) {
											res += "\t" + item.FullName;
											res += "\t" + item.BaseFullName;
											try { res += "\t" + item.ClassType.FullName; }
											catch (Exception) { res += "\tException"; }
											res += "\t" + item.Name;
											res += "\t" + item.RefName;
											res += "\t" + item.RefNameInstantiated;
											res += "\t" + item.AuxStr;
											res += "\t" + item.LevelInheritance;
											res += "\t" + item.TemplateNestingLevel;
											res += "\t" + item.TemplateParametersCount;
											res += "\t" + item.ClassNestingLevel;
											try { res += "\t" + item.ClassType.Namespace; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.BaseType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.Assembly; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.AssemblyQualifiedName; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.Attributes; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.ContainsGenericParameters; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.CustomAttributes; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.DeclaringMethod; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.DeclaringType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.GUID; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.GenericParameterAttributes; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.GenericParameterPosition; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.GenericTypeArguments; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.HasElementType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsAbstract; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsAnsiClass; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsArray; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsAutoClass; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsAutoLayout; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsByRef; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsByRefLike; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsCOMObject; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsClass; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsCollectible; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsConstructedGenericType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsContextful; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsEnum; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsExplicitLayout; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsFunctionPointer; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsGenericMethodParameter; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsGenericParameter; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsGenericType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsGenericTypeDefinition; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsGenericTypeParameter; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsImport; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsInterface; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsLayoutSequential; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsMarshalByRef; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNested; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNestedAssembly; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNestedFamANDAssem; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNestedFamORAssem; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNestedFamily; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNestedPrivate; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNestedPublic; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsNotPublic; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsPointer; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsPrimitive; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsPublic; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsSZArray; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsSealed; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsSecurityCritical; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsSecuritySafeCritical; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsSecurityTransparent; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsSignatureType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsSpecialName; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsTypeDefinition; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsUnicodeClass; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsUnmanagedFunctionPointer; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsValueType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsVariableBoundArray; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.IsVisible; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.MemberType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.MetadataToken; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.Module; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.ReflectedType; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.StructLayoutAttribute; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.TypeHandle; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.TypeInitializer; }
											catch (Exception) { res += "\tException"; }
											try { res += "\t" + item.ClassType.UnderlyingSystemType; }
											catch (Exception) { res += "\tException"; }
											res += "\r\n";
										}
										File.WriteAllText(saveFileDialog.FileName + ".txt", res, Encoding.UTF8);*/
										PB.Value = 0;
										MessageBox.Show(
											"Файл сохранён:\n" + saveFileDialog.FileName,
											"Готово",
											MessageBoxButtons.OK,
											MessageBoxIcon.Information
										);
									}
									catch (UnauthorizedAccessException ex) {
										MessageBox.Show(
											"Нет прав на запись в выбранную папку.\n" + ex.Message,
											"Ошибка доступа",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error
										);
									}
									catch (IOException ex) {
										MessageBox.Show(
											"Ошибка ввода-вывода.\n" + ex.Message,
											"Ошибка",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error
										);
									}
									catch (Exception ex) {
										MessageBox.Show(
											"Не удалось сохранить файл.\n" + ex.Message,
											"Ошибка",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error
										);
									}
								}
							}
						}
					}
					catch (Exception ex) {
						// Обработка возможных ошибок (например, файл занят или нет прав)
						MessageBox.Show("Ошибка при чтении файла: " + ex.Message,
										"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {

		}
	}
}
