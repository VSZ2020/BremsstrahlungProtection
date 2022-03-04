using System.Windows.Input;

namespace BSP
{
	public static class CustomCommands
	{
		public static RoutedCommand cmdRun = new RoutedUICommand("Options", "cmdRun", typeof(MainWindow),
			new InputGestureCollection(new InputGesture[]
			{
				new KeyGesture(Key.F5, ModifierKeys.None)
			}));

		public static RoutedCommand cmdStop = new RoutedUICommand("Options", "cmdStop", typeof(MainWindow),
			new InputGestureCollection(new InputGesture[]
			{
				new KeyGesture(Key.F6, ModifierKeys.None)
			}));

	}
}
