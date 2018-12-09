using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGitChecker {
    public class CheckResultModel {
        public string Type { set; get; }
        public string BranchName { set; get; }
        public string DisplayDir { set; get; }
        public string Dir { set; get; }
    }
}
