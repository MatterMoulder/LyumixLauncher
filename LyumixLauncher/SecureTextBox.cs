using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyumixLauncher
{
    public class SecureTextBox : TextBox
    {
        private bool _inSecure;

        public bool InSecure
        {
            get => _inSecure;
            set
            {
                _inSecure = value;
                if (_inSecure)
                {
                    // Change the password character to '*'
                    this.UseSystemPasswordChar = true;
                }
                else
                {
                    // Show the actual text
                    this.UseSystemPasswordChar = false;
                }
            }
        }

        public SecureTextBox()
        {
            _inSecure = false; // Default is not secure
        }
    }

}
