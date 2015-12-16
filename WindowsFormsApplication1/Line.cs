using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class Line : NetworkObject
    {
        public int StartX, StartY, EndX, EndY;

        public new static string networkNameIdentifier
        {
            get
            {
                return "Line";
            }
        }

        public Line(int networkID, int[] points = null ) : base(networkID)
        {
            SetPoints(points ?? new int[] { 0, 0, 0, 0 });
        }
        public void SetPoints(int[] points)
        {
            StartX = points[0];
            StartY = points[1];
            EndX = points[2];
            EndY = points[3];
        }
        public string GetCSVString()
        {
            return StartX + "," +
                StartY + "," +
                EndX + "," +
                EndY;
        }
    }
}
