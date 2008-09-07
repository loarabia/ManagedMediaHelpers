using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mp3MediaStreamSource
{
    public class Mp3MediaStreamSource: MediaStreamSource
    {

        protected override void CloseMedia()
        {
            throw new NotImplementedException();
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            throw new NotImplementedException();
        }

        protected override void OpenMediaAsync()
        {
            throw new NotImplementedException();
        }

        protected override void SeekAsync(long seekToTime)
        {
            throw new NotImplementedException();
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }
    }
}
