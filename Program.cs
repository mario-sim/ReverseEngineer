namespace ReverseEngineer {
	internal static class Program {
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			Lib.Logger.TLoggerContainer.VerbousLevel = Lib.Logger.EVerbousLevel.Trace;
			string appDirectory = AppContext.BaseDirectory;
			Lib.Logger.TLoggerContainer.Init(appDirectory + "\\NLog.config");
			ApplicationConfiguration.Initialize();
			Application.Run(new FRose());
		}
	}
}