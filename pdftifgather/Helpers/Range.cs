using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class Range
    {
        /// <summary>
        /// 1-
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// 1- ... int.MaxValue
        /// </summary>
        public int Last { get; set; }

        /// <summary>
        /// l,r,d
        /// </summary>
        public string Rot { get; set; }

        /// <summary>
        /// 右回転の度数
        /// 0,90,180,270 のいずれか
        /// </summary>
        public int Angle
        {
            get
            {
                switch (char.ToLowerInvariant((Rot + " ")[0]))
                {
                    case 'l': return 270;
                    case 'r': return 90;
                    case 'd': return 180;
                }
                return 0;
            }
        }
    }
}
