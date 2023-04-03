using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    class DrawnLine
    {
        private long firstEnd;
        private long secondEnd;
        private System.Windows.Shapes.Line otherLine;

        public long FirstEnd
        {
            get
            {
                return firstEnd;
            }

            set
            {
                firstEnd = value;
            }
        }
        public long SecondEnd
        {
            get
            {
                return secondEnd;
            }

            set
            {
                secondEnd = value;
            }
        }

        public System.Windows.Shapes.Line OtherLine
        {
            get
            {
                return otherLine;
            }

            set
            {
                otherLine = value;
            }
        }
    }
}
