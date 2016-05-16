using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Linq;

namespace MinifyOnSave
{
	public class OptionsPage : DialogPage
	{
		private const string CategoryName = "Minify On Save";

		#region Public Settings
		

		[Category(CategoryName)]
		[DisplayName("Enabled")]
		[Description("Determines whether a minify should triggered upon document save")]
		public bool Enabled
		{
			get
			{
				return Settings.AutoMinifyEnabled;
			}
			set
			{
				Settings.AutoMinifyEnabled = value;
			}
		}

		[Category(CategoryName)]
		[DisplayName("Extensions")]
		[Description("Document extensions which trigger a minify upon saving a document")]
		public string[] Extensions
		{
			get
			{
				return Settings.Extensions.ToArray();
			}
			set
			{
				Settings.Extensions = value;
			}
		}
		#endregion

	}
}
