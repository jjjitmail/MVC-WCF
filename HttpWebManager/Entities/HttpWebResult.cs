using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpWebManager
{
    public class HttpWebResult
    {
        private readonly string _Message;
        private string _ErrorMessage;
        private bool _AccessGranted;

        public bool IsSuccess { get; set; }
        public string Message
        {
            get
            {
                return IsSuccess ? "Success" : "Failed";
            }
        }
        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage ?? "Error onbekend";
            }
            set
            {
                if (_ErrorMessage == value)
                    return;
                _ErrorMessage = value;
            }
        }
        public bool AccessGranted
        {
            get
            {
                return _AccessGranted;
            }
            set
            {
                if (_AccessGranted == value)
                    return;
                _AccessGranted = value;
            }
        }
    }
}
