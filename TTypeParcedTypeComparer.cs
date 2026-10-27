using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseEngineer {
	public class TTypeParcedTypeComparer : Lib.RBT.IKeyEqualityComparer<TTypeParced> {
		public TTypeParcedTypeComparer() {
			//			_stringComparison = System.StringComparison.OrdinalIgnoreCase;
		}
		public int Compare(TTypeParced? x, TTypeParced? y) {
			if (x != null) {
				if (y != null) {
					string xc = x.ClassType.FullName??"";
					string yc = y.ClassType.FullName??"";
					return string.Compare(xc, yc, System.StringComparison.Ordinal);
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
				if (key is Type y) {
					string xc = x.ClassType.FullName??"";
					string yc = y.FullName??"";
					return string.Compare(xc, yc, System.StringComparison.Ordinal);
				}
				return 1;
			}
			if (key is TTypeParcedKey)
				return -1;
			return 0;
		}
		public object? MakeKey(TTypeParced data) {
			if (data != null)
				return data.ClassType;
			return null;
		}
	}
}
