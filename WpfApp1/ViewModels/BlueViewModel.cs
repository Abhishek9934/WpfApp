using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class BlueViewModel
    {
        public object header;

        public BlueViewModel(object header)
        {
            this.header = header;
            Console.WriteLine(header);
        }
    }
}
