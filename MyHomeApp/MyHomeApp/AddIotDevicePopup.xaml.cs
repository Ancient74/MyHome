using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHomeApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddIotDevicePopup : Popup, IDismissable
	{
		public AddIotDevicePopup ()
		{
			InitializeComponent ();
		}

        public void Dismiss()
        {
			this.Dismiss(null);
        }
    }
}