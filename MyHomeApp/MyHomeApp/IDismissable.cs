using System;
using System.Collections.Generic;
using System.Text;

namespace MyHomeApp
{
    internal interface IDismissable
    {
        void Dismiss();
    }

    internal interface IDismissable<T>
    {
        void Dismiss(T result);
    }
}
