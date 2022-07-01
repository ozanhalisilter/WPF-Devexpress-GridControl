using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Devexpress_GridControl.Model
{

    public class Person
    {
        public Person(int v1, string v2, string v3)
        {
            Id = v1;
            FirstName = v2;
            LastName = v3;
        }

        public int Id { get; set; } //Browesable attribute ?
        public string FirstName { get; set; }
        public string LastName { get; set; }
        private int Age { get; set; }
        private string Address { get; set; }


        private string FullName
        {
            get { return $"{FirstName} {LastName}"; }

        }

        private string V1 { get; }
        private string V2 { get; }
        private int V3 { get; }
        private string V4 { get; }


        public override string ToString()
        {
            return $"{FullName} {Age} {Address}";
        }
    }



}
