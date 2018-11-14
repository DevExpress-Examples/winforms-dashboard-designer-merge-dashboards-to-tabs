using System;
using System.Collections.Generic;
using System.Linq;

namespace DashboardMerger {
    public static class NamesGenerator {
        public static string GenerateName(string name, int index, IEnumerable<string> occupiedNames) {
            string result = String.Format("{0}_{1}", name, index);
            if(occupiedNames.Contains(result))
                return GenerateName(name, ++index, occupiedNames);
            return result;
        }
    }
}
