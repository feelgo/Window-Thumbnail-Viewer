﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowThumbViewer
{
	public partial class MainWindow : Window
	{
		private static SettingsWindow Settings;
		public static Dictionary<IntPtr, string> AllHandles = new Dictionary<IntPtr, string>();
		public static ObservableCollection<ListItemTemplate> SelectedHandles { get; set; }
		public static IntPtr Handle;

		public MainWindow()
		{
			InitializeComponent();
			Handle = new WindowInteropHelper( this ).Handle;
			Settings = new SettingsWindow();
			LoadWindows();
		}

		private List<Rect> CreatePreset( int i )
		{
			var left = 0;
			var top = 0;
			var right = (int)this.ActualWidth-15;
			var bottom = (int)this.ActualHeight-38;

			var ret = new List<Rect>();
			var mainRect = new Rect( left, top, right, bottom );
			switch( i )
			{
				case 1:
					{
						ret.Add( mainRect );
						break;
					}
				case 2:
					{
						ret = mainRect.AsList().SplitHorizontaly().SortRects();
						break;
					}
				case 3:
					{
						var verticals = mainRect.AsList().SplitVertically();
						var firstLine = verticals.Take(1).ToList().SplitHorizontaly();
						var secondLine = verticals.Skip(1).ToList();
						ret = firstLine.Concat( secondLine ).ToList().SortRects();
						break;
					}
				case 4:
					{
						ret = mainRect.AsList().SplitVertically().SplitHorizontaly().SortRects();
						break;
					}
				case 5:
					{
						var verticals = mainRect.AsList().SplitVertically();
						var firstLine = verticals.Take(1).ToList().SplitHorizontaly(3);
						var secondLine = verticals.Skip(1).ToList().SplitHorizontaly(2);
						ret = firstLine.Concat( secondLine ).ToList().SortRects();
						break;
					}
				case 6:
					{
						ret = mainRect.AsList().SplitHorizontaly( 3 ).SplitVertically().SortRects();
						break;
					}
				case 7:
					{
						var verticals = mainRect.AsList().SplitVertically(3);
						var firstLine = verticals.Take(1).ToList().SplitHorizontaly(3);
						var otherLines = verticals.Skip(1).ToList().SplitHorizontaly(2);
						ret = firstLine.Concat( otherLines ).ToList().SortRects();
						break;
					}
				case 8:
					{
						var verticals = mainRect.AsList().SplitVertically(3);
						var firstLines = verticals.Take(2).ToList().SplitHorizontaly(3);
						var lastLine = verticals.Skip(2).ToList().SplitHorizontaly(2);
						ret = firstLines.Concat( lastLine ).ToList().SortRects();
						break;
					}
				case 9:
					{
						ret = mainRect.AsList().SplitHorizontaly( 3 ).SplitVertically( 3 ).SortRects();
						break;
					}
				case 10:
					{
						var verticals = mainRect.AsList().SplitVertically(4);
						var firstLines = verticals.Take(2).ToList().SplitHorizontaly(3);
						var lastLines = verticals.Skip(2).ToList().SplitHorizontaly(2);
						ret = firstLines.Concat( lastLines ).ToList().SortRects();
						break;
					}
				case 11:
					{
						var verticals = mainRect.AsList().SplitVertically(4);
						var firstLines = verticals.Take(3).ToList().SplitHorizontaly(3);
						var lastLines = verticals.Skip(3).ToList().SplitHorizontaly(2);
						ret = firstLines.Concat( lastLines ).ToList().SortRects();
						break;
					}
				case 12:
					{
						ret = mainRect.AsList().SplitHorizontaly( 3 ).SplitVertically( 4 ).SortRects();
						break;
					}
				case 13:
					{
						var verticals = mainRect.AsList().SplitVertically(4);
						var firstLine = verticals.Take(1).ToList().SplitHorizontaly(4);
						var otherLines = verticals.Skip(1).ToList().SplitHorizontaly(3);
						ret = firstLine.Concat( otherLines ).ToList().SortRects();
						break;
					}
				case 14:
					{
						var verticals = mainRect.AsList().SplitVertically(4);
						var firstLine = verticals.Take(2).ToList().SplitHorizontaly(4);
						var otherLines = verticals.Skip(2).ToList().SplitHorizontaly(3);
						ret = firstLine.Concat( otherLines ).ToList().SortRects();
						break;
					}
				case 15:
					{
						var verticals = mainRect.AsList().SplitVertically(4);
						var firstLine = verticals.Take(3).ToList().SplitHorizontaly(4);
						var otherLines = verticals.Skip(3).ToList().SplitHorizontaly(3);
						ret = firstLine.Concat( otherLines ).ToList().SortRects();
						break;
					}
				case 16:
					{
						ret = mainRect.AsList().SplitHorizontaly( 4 ).SplitVertically( 4 ).SortRects();
						break;
					}
			}

			return ret;
		}

		private void Window_KeyUp( object sender, KeyEventArgs e )
		{
			switch( e.Key )
			{
				case Key.F1: HandleSettings(); break;
				case Key.F5: HandleRefreshAll(); break;
			}
		}

		public static void LoadWindows()
		{
			AllHandles.Clear();

			Win32Funcs.EnumWindows( Callback, 0 );
		}

		private static bool Callback( IntPtr hwnd, int lParam )
		{
			if( Handle == hwnd || Settings.Handle == hwnd )
				return true;

			if( ( Win32Funcs.GetWindowLongA( hwnd, Win32Funcs.GWL_STYLE ) & Win32Funcs.TARGETWINDOW ) == Win32Funcs.TARGETWINDOW )
			{
				StringBuilder sb = new StringBuilder(100);
				Win32Funcs.GetWindowText( hwnd, sb, sb.Capacity );
				AllHandles.Add( hwnd, sb.ToString() );
			}

			return true; //continue enumeration
		}

		private void HandleSettings()
		{
			Settings.Close();
			Settings = new SettingsWindow();
			var maximized = this.WindowState.HasFlag(WindowState.Maximized);
			var left = maximized ? 0 : this.Left;
			var top = maximized ? 0 : this.Top;
			Settings.Left = left + ( this.ActualWidth - Settings.Width ) / 2;
			Settings.Top = top + ( this.ActualHeight - Settings.Height ) / 2;
			Settings.Show();
		}

		private void HandleRefreshAll()
		{
			Handle = new WindowInteropHelper( this ).Handle;
			var preset = CreatePreset(SelectedHandles.Count);
			var scale = GetSystemScale();
			var scaled = preset.Select( ( x ) => x.Scale( scale ) ).ToList();
			canvas.Children.Clear();

			foreach( var win in SelectedHandles )
			{
				Rectangle rect = new Rectangle();
				rect.Width = preset[win.Id].Right - preset[win.Id].Left;
				rect.Height = preset[win.Id].Bottom - preset[win.Id].Top;

				rect.Stroke = Brushes.Black;
				rect.StrokeThickness = 3;

				canvas.Children.Add( rect );
				Canvas.SetTop( rect, preset[win.Id].Top );
				Canvas.SetLeft( rect, preset[win.Id].Left );

				Label lab = new Label();
				lab.FontSize = 50;
				lab.Content = win.Id + 1;
				canvas.Children.Add( lab );
				Canvas.SetTop( lab, preset[win.Id].Top );
				Canvas.SetLeft( lab, preset[win.Id].Left );


				if( win.ThumbnailHandle != IntPtr.Zero )
					Win32Funcs.DwmUnregisterThumbnail( win.ThumbnailHandle );

				IntPtr handle;

				int ret = Win32Funcs.DwmRegisterThumbnail(Handle, win.Handle, out handle);
				win.ThumbnailHandle = handle;

				if( ret == 0 )
				{
					PSIZE size;
					Win32Funcs.DwmQueryThumbnailSourceSize( win.ThumbnailHandle, out size );

					//this.Title = $"Actual:{Width}/{Height} Preset:{preset[win.Id].Left}/{preset[win.Id].Top},{preset[win.Id].Right}/{preset[win.Id].Bottom} Size:{size.x}/{size.y}";


					DWM_THUMBNAIL_PROPERTIES props = new DWM_THUMBNAIL_PROPERTIES();

					props.dwFlags =
						Win32Funcs.DWM_TNP_SOURCECLIENTAREAONLY |
						Win32Funcs.DWM_TNP_VISIBLE |
						Win32Funcs.DWM_TNP_OPACITY |
						Win32Funcs.DWM_TNP_RECTDESTINATION;

					props.fSourceClientAreaOnly = false;
					props.fVisible = true;
					props.opacity = 255;
					props.rcDestination = scaled[win.Id];

					Win32Funcs.DwmUpdateThumbnailProperties( win.ThumbnailHandle, ref props );
				}

			}
		}

		public double GetSystemScale()
		{
			var dpi = 1.0;
			using( System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd( IntPtr.Zero ) )
			{
				dpi = graphics.DpiX / 96.0;
			}
			return dpi;
		}

		private void Window_MouseMove( object sender, MouseEventArgs e )
		{
			//this.Title = e.GetPosition( this ).ToString();
		}
	}
}