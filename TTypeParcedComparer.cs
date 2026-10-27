using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseEngineer {
	public class TTypeParcedKey {
		public string FullName="";
		public int ClassNestingLevel;
		public int TemplateNestingLevel;
		public int TemplateParametersCount;
		public string AuxStr="";
		public int LevelInheritance;
	}
	public class TTypeParcedComparer : Lib.RBT.IKeyEqualityComparer<TTypeParced> {
		public TTypeParcedComparer() {
			//			_stringComparison = System.StringComparison.OrdinalIgnoreCase;
		}
		public int Compare(TTypeParced? x, TTypeParced? y) {
			if (x != null) {
				if (y != null) {
					int xc = x.ClassNestingLevel;
					int yc = y.ClassNestingLevel;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					xc = x.TemplateNestingLevel;
					yc = y.TemplateNestingLevel;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					xc = x.TemplateParametersCount;
					yc = y.TemplateParametersCount;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					int i = string.Compare(x.AuxStr, y.AuxStr, System.StringComparison.Ordinal);
					if (i != 0)
						return i;
					i = string.Compare(x.FullName, y.FullName, System.StringComparison.Ordinal);
					if (i != 0)
						return i;
					xc = x.LevelInheritance;
					yc = y.LevelInheritance;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					return 0;
				}
				return 1;
			}
			if (y != null)
				return -1;
			return 0;
		}
		public bool Equals(TTypeParced x, TTypeParced y) {
			return Compare(x, y) == 0;
		}
		public int CompareToKey(TTypeParced? x, object? key) {
			if (x != null) {
				if (key is TTypeParcedKey y) {
					int xc = x.ClassNestingLevel;
					int yc = y.ClassNestingLevel;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					xc = x.TemplateNestingLevel;
					yc = y.TemplateNestingLevel;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					xc = x.TemplateParametersCount;
					yc = y.TemplateParametersCount;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					int i = string.Compare(x.AuxStr, y.AuxStr, System.StringComparison.Ordinal);
					if (i != 0)
						return i;
					i = string.Compare(x.FullName, y.FullName, System.StringComparison.Ordinal);
					if (i != 0)
						return i;
					xc = x.LevelInheritance;
					yc = y.LevelInheritance;
					if (xc < yc)
						return -1;
					if (xc > yc)
						return 1;
					return 0;
				}
				return 1;
			}
			if (key is TTypeParcedKey)
				return -1;
			return 0;
		}
		public object? MakeKey(TTypeParced data) {
			if (data != null)
				return new TTypeParcedKey() {
					ClassNestingLevel = data.ClassNestingLevel,
					TemplateNestingLevel = data.TemplateNestingLevel,
					TemplateParametersCount = data.TemplateParametersCount,
					AuxStr = data.AuxStr,
					LevelInheritance = data.LevelInheritance,
					FullName = data.FullName
				};
			return null;
		}
	}
}
