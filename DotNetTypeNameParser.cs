using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseEngineer {
	public sealed class TypeRef {
		public TypeRef() { }
		public TypeRef(string name) { Name = name; }

		public string Name { get; set; } = "";
		public List<TypeRef>? GenericArgs { get; set; }
		public int TemplateNestingLevel {
			get {
				if (GenericArgs == null)
					return 0;
				int i=1;
				int k=0;
				foreach (var item in GenericArgs) {
					if (item.TemplateNestingLevel > k)
						k = item.TemplateNestingLevel;
				}
				return i+k;
			}
		}
		public AssemblyRef? Assembly { get; set; }
	}

	public sealed class AssemblyRef {
		public AssemblyRef() { }
		public AssemblyRef(string name) { Name = name; }

		public string Name { get; set; } = "";
		public string? Version { get; set; }
		public string? Culture { get; set; }
		public string? PublicKeyToken { get; set; }
	}
	public static class DotNetTypeNameParser {
		public static TypeRef Parse(string input) {
			var tokens = Tokenize(input);
			var p = new Parser(tokens);
			var r = p.Parse();
			return r;
		}

		// ---- Токенизация ----
		private static List<string> Tokenize(string s) {
			var tokens = new List<string>();
			var sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++) {
				char c = s[i];
				if (c == '\\' && i + 1 < s.Length) // \] \, \\ и т.п.
				{
					sb.Append(s[i + 1]);
					i++;
					continue;
				}
				if (c is '[' or ']' or ',') {
					if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
					tokens.Add(c.ToString());
					continue;
				}
				if (char.IsWhiteSpace(c)) {
					if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
					continue;
				}
				sb.Append(c);
			}
			if (sb.Length > 0)
				tokens.Add(sb.ToString());
			return tokens;
		}

		// ---- Разбор ----
		private sealed class Parser {
			private readonly List<string> _t;
			private int _i;
			public Parser(List<string> t) { _t = t; }

			public TypeRef Parse() {
				var r = ParseTypeRef();
				if (_i != _t.Count)
					throw new FormatException($"Лишние токены: '{_t[_i]}' (позиция {_i})");
				return r;
			}

			// Полный TypeRef: имя + (generic args) + (, assembly-qualifier)
			private TypeRef ParseTypeRef() {
				var tr = new TypeRef( ) { Name = Next() };

				if (Peek() == "[") {
					Next(); // [
					tr.GenericArgs = new List<TypeRef>();
					if (Peek() != "]") {
						bool wrapped = Peek() == "[";
						if (wrapped) {
							do {
								Expect("[");
								tr.GenericArgs.Add(ParseTypeRef());
								Expect("]");
							} while (TryConsume(","));
						}
						else {
							tr.GenericArgs.Add(ParseTypeRefBare());
							while (TryConsume(","))
								tr.GenericArgs.Add(ParseTypeRefBare());
						}
					}
					Expect("]");
				}

				if (TryConsume(","))
					tr.Assembly = ParseAssemblyRef();

				return tr;
			}

			// Для «голых» generic-аргументов: без сборки
			private TypeRef ParseTypeRefBare() {
				var tr = new TypeRef { Name = Next() };
				if (Peek() == "[") {
					Next();
					tr.GenericArgs = new List<TypeRef>();
					if (Peek() != "]") {
						tr.GenericArgs.Add(ParseTypeRefBare());
						while (TryConsume(","))
							tr.GenericArgs.Add(ParseTypeRefBare());
					}
					Expect("]");
				}
				return tr;
			}

			private AssemblyRef ParseAssemblyRef() {
				var a = new AssemblyRef { Name = Next() };
				while (TryConsume(",")) {
					string kv = Next();
					int eq = kv.IndexOf('=');
					if (eq <= 0)
						continue;
					string key = kv[..eq];
					string val = kv[(eq + 1)..];
					switch (key) {
						case "Version":
							a.Version = val;
							break;
						case "Culture":
							a.Culture = val;
							break;
						case "PublicKeyToken":
							a.PublicKeyToken = val;
							break;
					}
				}
				return a;
			}

			private string? Peek() => _i < _t.Count ? _t[_i] : null;
			private string Next() => _t[_i++];
			private void Expect(string tok) {
				if (!TryConsume(tok))
					throw new FormatException($"Ожидался '{tok}' на позиции {_i}, а получен '{(Peek() ?? "<eof>")}'");
			}
			private bool TryConsume(string tok) {
				if (Peek() == tok) { _i++; return true; }
				return false;
			}
		}
	}
}