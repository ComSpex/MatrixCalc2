// $Header: /ComSpex/MatrixCalc.root/MatrixCalc/Window1.xaml.cs 19    13/10/29 15:29 Yosuke $
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.RegularExpressions;

namespace MatrixCalc {
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1:Window {
		Dictionary<string,Transform> dic;
		Viewbox vbox=new Viewbox();
		public const string double_format="{0,7:0.00}";
		bool _dirty=false;
		public Window1() {
			this.SizeChanged+=new SizeChangedEventHandler(Window1_SizeChanged);
			this.Closed+=new EventHandler(Window1_Closed);
			InitializeComponent();
			
			dic=new Dictionary<string,Transform>(){
				{"stR",this.stR},
				{"sqR",this.sqR},
				{"ttR",this.ttR},
				{"rtR",this.rtR},
				{"mtR",this.mtR},
				{"stL",this.stL},
				{"sqL",this.sqL},
				{"ttL",this.ttL},
				{"rtL",this.rtL},
				{"mtL",this.mtL},
			};
			
			UIElement content=this.Content as UIElement;
			this.Content=null;
			vbox.Child=content;
			this.Content=vbox;
			
			DispatcherTimer layout=new DispatcherTimer();
			layout.Interval=TimeSpan.FromSeconds(0.5);
			layout.Tick+=new EventHandler(layout_Tick);
			layout.Start();
		}
		void Window1_Closed(object sender,EventArgs e) {
			//Clipboard.Clear();
		}
		void Window1_SizeChanged(object sender,SizeChangedEventArgs e) {
			System.Diagnostics.Debug.WriteLine(String.Format("{0} x {1}",e.NewSize.Width,e.NewSize.Height));
		}
		void layout_Tick(object sender,EventArgs e) {
			DispatcherTimer timer=sender as DispatcherTimer;
			ResetAll();
			timer.Stop();
		}
		/// <summary>
		/// http://www.teacherschoice.com.au/maths_library/angles/angles.htm
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		double ToRadians(double degrees){
			return degrees*Math.PI/180.0;
		}
		double ToDegrees(double radians){
			return radians*180.0/Math.PI;
		}
		string f(double value){
			return String.Format(double_format,value);
		}
		private void TextBox_TextChanged(object sender,TextChangedEventArgs e) {
			TextBox tb=sender as TextBox;
			if(String.IsNullOrEmpty(tb.Text)) {
				return;
			}
			if(tb.Text.StartsWith("-")){
				Match Ma=Regex.Match(tb.Text,"[-][0-9]+");
				if(!Ma.Success){
					return;
				}
			}else if(tb.Text.StartsWith("+")){
				Match Ma=Regex.Match(tb.Text,"[+][0-9]+");
				if(!Ma.Success) {
					return;
				}
			} else {
				Match Ma=Regex.Match(tb.Text,"[0-9]+");
				if(!Ma.Success) {
					return;
				}
			}
			switch(tb.Name.Substring(0,1)) {
				case "a":{
					Handle_SkewTransform();
					}break;
				case "r":{
					Handle_RenderTransformOrigin();
					}break;
				case "m":{
					Handle_MatrixTransform();
					} break;
				case "o":goto case "m";
				case "A":{
					Handle_RotateTransform();
					}break;
				case "c":{
					Handle_CenterXY();
					}break;
				case "s":{
					Handle_ScaleTransform();
					}break;
				case "d":
					Handle_TranslateTransform();
					break;
			}
		}
		private void Handle_SkewTransform() {
			try {
				double ax=double.Parse(this.aX.Text);
				double ay=double.Parse(this.aY.Text);
				#if false
				// Note that is Tangent!!
				this.lm21.Content=f(Math.Tan(ToRadians(ax)));
				this.lm12.Content=f(Math.Tan(ToRadians(ay)));
				#endif
				if(this.Rt.IsChecked.Value) {
					this.sqR.AngleX=ax;
					this.sqR.AngleY=ay;
				} else {
					this.sqL.AngleX=ax;
					this.sqL.AngleY=ay;
				}
			} catch(Exception) {
				this.sqR.CenterX=this.sqR.CenterY=
				this.sqL.CenterX=this.sqL.CenterY=
				this.sqR.AngleX=
				this.sqR.AngleY=
				this.sqL.AngleX=
				this.sqL.AngleY=0.0;
				this.aX.Text=
				this.aY.Text="0";
			}
		}
		void Handle_RenderTransformOrigin(){
			try {
				double x=double.Parse(this.rtoX.Text);
				double y=double.Parse(this.rtoY.Text);
				x=Math.Min(Math.Max(x,0.0),1.0);
				y=Math.Min(Math.Max(y,0.0),1.0);
				this.example.RenderTransformOrigin=new Point(x,y);
				Canvas.SetLeft(this.rtorigi,Canvas.GetLeft(this.example)+(this.example.ActualWidth)*x-this.rtorigi.ActualWidth/2.0);
				Canvas.SetTop(this.rtorigi,Canvas.GetTop(this.example)+(this.example.ActualHeight)*y-this.rtorigi.ActualHeight/2.0);
			} catch(Exception) {
				this.rtoX.Text=this.rtoY.Text="0";
				this.example.RenderTransformOrigin=new Point();
				Canvas.SetLeft(this.rtorigi,Canvas.GetLeft(this.example)-this.rtorigi.ActualWidth/2.0);
				Canvas.SetTop(this.rtorigi,Canvas.GetTop(this.example)-this.rtorigi.ActualHeight/2.0);
			}
		}
		private void Handle_MatrixTransform() {
			try {
				double m11=double.Parse(this.m11.Text);
				double m12=double.Parse(this.m12.Text);
				double m21=double.Parse(this.m21.Text);
				double m22=double.Parse(this.m22.Text);
				double ox=double.Parse(this.oX.Text);
				double oy=double.Parse(this.oY.Text);
				if(this.Rt.IsChecked.Value) {
					this.mtR.Matrix=new Matrix(m11,m12,m21,m22,ox,oy);
				} else {
					this.mtL.Matrix=new Matrix(m11,m12,m21,m22,ox,oy);
				}
			} catch(Exception) {
				this.m11.Text=this.m22.Text="1";
				this.m12.Text=this.m21.Text=this.oX.Text=this.oY.Text="0";
				this.mtR.Matrix=new Matrix();
				this.mtL.Matrix=new Matrix();
			}
		}
		private void Handle_TranslateTransform() {
			try{
				double dx=double.Parse(this.dX.Text);
				double dy=double.Parse(this.dY.Text);
				if(this.Rt.IsChecked.Value){
					this.ttR.X=dx;
					this.ttR.Y=dy;
				}else{
					this.ttL.X=dx;
					this.ttL.Y=dy;
				}
			} catch(Exception) {
				this.ttR.X=
				this.ttR.Y=
				this.ttL.X=
				this.ttL.Y=0.0;
			}
		}
		private void Handle_ScaleTransform() {
			try{
				double sx=double.Parse(this.sX.Text);
				double sy=double.Parse(this.sY.Text);
				if(this.Rt.IsChecked.Value) {
					this.stR.ScaleX=sx;
					this.stR.ScaleY=sy;
				} else {
					this.stL.ScaleX=sx;
					this.stL.ScaleY=sy;
				}
			}catch(Exception){
				this.stR.ScaleX=
				this.stR.ScaleY=
				this.stL.ScaleX=
				this.stL.ScaleY=1.0;
			}
		}
		void Handle_RotateTransform(){
			try {
				double degrees=double.Parse(this.Angle.Text);
				double radians=ToRadians(degrees);
				#if false
				this.lm11.Content=f(Math.Cos(radians));
				this.lm12.Content=f(Math.Sin(radians));
				this.lm21.Content=f(Math.Sin(radians)*-1.0);
				this.lm22.Content=f(Math.Cos(radians));
				#endif
				try {
					if(this.Rt.IsChecked.Value) {
						this.rtR.Angle=degrees;
						this.rtR.CenterX=center_x;
						this.rtR.CenterY=center_y;
					} else {
						this.rtL.Angle=degrees;
						this.rtL.CenterX=center_x;
						this.rtL.CenterY=center_y;
					}
					DoEvents();
				} catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			} catch(Exception) {
				this.rtR.Angle=this.rtL.Angle=0.0;
			}
		}
		double center_x{get;set;}
		double center_y{get;set;}
		void Handle_CenterXY(){
			try {
				center_x=double.Parse(this.cX.Text);
				center_y=double.Parse(this.cY.Text);
				if(this.Rt.IsChecked.Value) {
					this.sqR.CenterX=
					this.stR.CenterX=
					this.rtR.CenterX=center_x;
					this.sqR.CenterY=
					this.stR.CenterY=
					this.rtR.CenterY=center_y;
				} else {
					this.sqL.CenterX=
					this.stL.CenterX=
					this.rtL.CenterX=center_x;
					this.sqL.CenterY=
					this.stL.CenterY=
					this.rtL.CenterY=center_y;
				}
				Canvas.SetLeft(this.rorigin,center_x-this.rorigin.ActualWidth/2.0);
				Canvas.SetTop(this.rorigin,center_y-this.rorigin.ActualHeight/2.0);
			} catch(Exception) {
				this.stR.CenterX=this.stR.CenterY=this.stL.CenterX=this.stL.CenterY=
				this.rtR.CenterX=this.rtR.CenterY=this.rtL.CenterX=this.rtL.CenterY=0.0;
				double cx=Canvas.GetLeft(this.example)+(this.example.ActualWidth-this.rorigin.ActualWidth)/2.0;
				double cy=Canvas.GetTop(this.example)+(this.example.ActualHeight-this.rorigin.ActualHeight)/2.0;
				Canvas.SetLeft(this.rorigin,cx);
				Canvas.SetTop(this.rorigin,cy);
			}
		}
		virtual public void DoEvents() {
			try {
				DispatcherFrame frame=new DispatcherFrame();
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,new DispatcherOperationCallback(ExitFrame),frame);
				Dispatcher.PushFrame(frame);
			} catch(Exception ex) {
				throw ex;
			}
		}
		virtual protected object ExitFrame(object f) {
			try {
				((DispatcherFrame)f).Continue=false;
				return null;
			} catch(Exception ex) {
				throw ex;
			}
		}
		void ResetAll(){
			this.example.RenderTransformOrigin=new Point();
			Canvas.SetLeft(this.rtorigi,Canvas.GetLeft(this.example)-this.rtorigi.ActualWidth/2.0);
			Canvas.SetTop(this.rtorigi,Canvas.GetTop(this.example)-this.rtorigi.ActualHeight/2.0);

			this.oX.Text="0";
			this.oY.Text="0";
			Canvas.SetLeft(this.rorigin,-1*this.rorigin.ActualWidth/2.0);
			Canvas.SetTop(this.rorigin,-1*this.rorigin.ActualHeight/2.0);
			
			this.sX.Text=this.sY.Text="1";
			this.dX.Text=this.dY.Text=
			this.rtoX.Text=this.rtoY.Text=
			this.Angle.Text=this.cX.Text=this.cY.Text="0";
			
			this.Rt.IsChecked=true;

			this.lm12.Content=
			this.lm21.Content=
			this.loX.Content=
			this.loY.Content=f(0);
			this.lm11.Content=
			this.lm22.Content=f(1);
			
			this.delta.Value=1.0;

			this.aX.Text=
			this.aY.Text="0";

			foreach(Transform trans in dic.Values){
				trans.Value.SetIdentity();
			}
			this.mtR.Matrix=new Matrix();
			this.mtL.Matrix=new Matrix();
			
			center_x=center_y=0.0;
			
			_dirty=false;
		}
		private void TextBox_PreviewKeyDown(object sender,KeyEventArgs e) {
			_dirty=true;
			if(e.Key==Key.Up) {
				HandleNumericUpDown(sender as TextBox,false);
				DoEvents();
				e.Handled=true;
			}
			if(e.Key==Key.Down) {
				HandleNumericUpDown(sender as TextBox,true);
				DoEvents();
				e.Handled=true;
			}
		}
		bool NotAnyOtherKeys{
			get{
				return !(
					Keyboard.IsKeyDown(Key.RightShift)||
					Keyboard.IsKeyDown(Key.LeftShift)||
					Keyboard.IsKeyDown(Key.RightCtrl)||
					Keyboard.IsKeyDown(Key.LeftCtrl)
				);
			}
		}
		void HandleNumericUpDown(TextBox tb,bool isUp){
			double value=double.NaN;
			if(String.IsNullOrEmpty(tb.Text)) {
				tb.Tag=0.0;
			} else {
				try {
					tb.Tag=double.Parse(tb.Text);
				} catch(Exception) {
					tb.Tag=0.0;
				}
			}
			value=(double)tb.Tag;
			if(isUp){
				value+=delta.Value;
			}else{
				value-=delta.Value;
			}
			tb.Text=value.ToString();
			tb.Tag=value;
		}

		private void rorigin_MouseDown(object sender,MouseButtonEventArgs e) {
			DragDrop.DoDragDrop(this.rorigin,sender,DragDropEffects.Move);
		}
		bool IsShiftPressed{
			get{
				return (Keyboard.IsKeyDown(Key.RightShift)||Keyboard.IsKeyDown(Key.LeftShift));
			}
		}
		private void delta_PreviewKeyDown(object sender,KeyEventArgs e) {
			#if false
			Slider slide=sender as Slider;
			if(IsShiftPressed){
				if(slide.Tag==null) {
					slide.Tag=0;
				}
				int index=(int)slide.Tag;
				if(index>=slide.Ticks.Count){
					index=0;
				}
				if(index<0){
					index=slide.Ticks.Count-1;
				}
				if(e.Key==Key.Right){
					slide.Value=slide.Ticks[index++];
				}else if(e.Key==Key.Left){
					slide.Value=slide.Ticks[index--];
				}
				slide.Tag=index;
			}
			#endif
		}
		private void GroupBox_GotFocus(object sender,RoutedEventArgs e) {
			GroupBox gb=sender as GroupBox;
			string header=(string)gb.Header;
			if(header.EndsWith("Origin")){
				gb.Tag=delta.Value;
				if(delta.Value>0.1) {
					delta.Value=0.1;
				}
			}
		}
		private void GroupBox_LostFocus(object sender,RoutedEventArgs e) {
			GroupBox gb=sender as GroupBox;
			string header=(string)gb.Header;
			if(header.EndsWith("Origin")){
				if(gb.Tag!=null){
					delta.Value=(double)gb.Tag;
				}
			}
		}

		double f(Label la){
			return double.Parse((string)la.Content);
		}
		Matrix ToMatrix(Label m11,Label m12,Label m21,Label m22,Label ox,Label oy){
			return new Matrix(f(m11),f(m12),f(m21),f(m22),f(ox),f(oy));
		}
		void FromMatrix(Matrix mat){
			this.lm11.Content=f(mat.M11);
			this.lm12.Content=f(mat.M12);
			this.lm21.Content=f(mat.M21);
			this.lm22.Content=f(mat.M22);
			this.loX.Content=f(mat.OffsetX);
			this.loY.Content=f(mat.OffsetY);
		}
		private void Button_Click(object sender,RoutedEventArgs e) {
			if(!_dirty){
				Matrix mat=ToMatrix(lm11,lm12,lm21,lm22,loX,loY);
				if(mat.IsIdentity){
					string text=Clipboard.GetText();
					if(!String.IsNullOrEmpty(text)){
						string[] numbers=text.Split(',');
						if(numbers.Length==6){
							this.mtR.Matrix=Matrix.Parse(text);
							FromMatrix(this.mtR.Value);
						}
					}
					return;
				}
			}
			Matrix total=new Matrix();
			foreach(KeyValuePair<string,Transform> pair in dic){
				if(pair.Value.Value.IsIdentity){
					continue;
				}
				total*=pair.Value.Value;
				System.Diagnostics.Debug.WriteLine(String.Format("{0}={1}",pair.Key,pair.Value.Value.ToString()));
			}
			System.Diagnostics.Debug.WriteLine(String.Format("Clipboard={0}",total.ToString()));
			Clipboard.SetText(String.Format("{0}",total.ToString()));
			FromMatrix(total);
			//MessageBox.Show(String.Format(@"Matrix=""{0}"" has been sent to Clipboard.",total.ToString()));
		}

		private void TextBox_GotFocus(object sender,RoutedEventArgs e) {
			TextBox tb=sender as TextBox;
			tb.SelectAll();
		}
		private void TextBox_GotMouseCapture(object sender,MouseEventArgs e) {
			TextBox tb=sender as TextBox;
			tb.SelectAll();
		}

		private void ResetAll_Click(object sender,RoutedEventArgs e) {
			if(MessageBoxResult.OK==MessageBox.Show("Are you sure?",this.Title,MessageBoxButton.OKCancel,MessageBoxImage.Question)) {
				ResetAll();
				DoEvents();
				ResetAll();
			}
		}
	}
}
