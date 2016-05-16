using Microsoft.VisualStudio.Shell.Interop;

namespace MinifyOnSave
{
	public static class Icons
	{
		/// <summary>
		///	Standard animation icon.
		/// </summary>
		public static short General => (short)Constants.SBAI_General;
		/// <summary>
		///	Animation when printing.
		/// </summary>
		public static short Print => (short)Constants.SBAI_Print;
		/// <summary>
		/// Animation when saving files.
		/// </summary>
		public static short Save => (short)Constants.SBAI_Save;
		/// <summary>
		/// Animation when deploying the solution.
		/// </summary>
		public static short Deploy => (short)Constants.SBAI_Deploy;
		/// <summary>
		/// Animation when synchronizing files over the network.
		/// </summary>
		public static short Sync => (short)Constants.SBAI_Synch;
		/// <summary>
		/// Animation when building the solution.
		/// </summary>
		public static short Build => (short)Constants.SBAI_Build;
		/// <summary>
		/// Animation when searching.
		/// </summary>
		public static short Find => (short)Constants.SBAI_Find;
	}
}
