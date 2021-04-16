// $Header: /ComSpex/MatrixCalc.root/MatrixCalc/Window1.DragnDrop.cs 4     13/10/29 15:20 Yosuke $
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MatrixCalc {
	public partial class Window1 {
		Dictionary<string,bool> droppedWas=new Dictionary<string,bool>();
		protected override void OnDragEnter(DragEventArgs e) {
			System.Diagnostics.Debug.WriteLine(DataFormatToDrop(e),"OnDragEnter");
			e.Effects=DragDropEffects.All;
			base.OnDragEnter(e);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			e.Action=DragAction.Drop;
			base.OnQueryContinueDrag(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			base.OnDragOver(e);
			object sender=e.Data.GetData(typeof(System.Windows.Shapes.Ellipse));
			if(sender!=null){
				Point p=e.GetPosition(this.canva);
				Shape elem=sender as Shape;
				Canvas.SetLeft(elem,p.X-elem.ActualWidth/2.0);
				Canvas.SetTop(elem,p.Y-elem.ActualHeight/2.0);
				this.cX.Text=f(p.X);
				this.cY.Text=f(p.Y);
			}
		}
		protected override void OnDrop(DragEventArgs e) {
			base.OnDrop(e);
			try{
				System.Diagnostics.Debug.WriteLine(DataFormatToDrop(e),"Drop");
				object sender=e.Data.GetData(typeof(System.Windows.Shapes.Ellipse));
				System.Diagnostics.Debug.WriteLine(sender.ToString());
				Point p=e.GetPosition(this.canva);
				Shape elem=sender as Shape;
				Canvas.SetLeft(elem,p.X-elem.ActualWidth/2.0);
				Canvas.SetTop(elem,p.Y-elem.ActualHeight/2.0);
				this.cX.Text=f(center_x=p.X);
				this.cY.Text=f(center_y=p.Y);
				_dirty=true;
			}catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}
		virtual protected string DataFormatToDrop(DragEventArgs e){
			TraverseDataFormats(e);
			foreach(KeyValuePair<string,bool> pair in droppedWas){
				if(pair.Value){
					return pair.Key;
				}
			}
			return string.Empty;
		}
		virtual protected void TraverseDataFormats(DragEventArgs e) {
			droppedWas.Clear();
			FieldInfo[] props=typeof(DataFormats).GetFields(BindingFlags.Static|BindingFlags.Public);
			foreach(FieldInfo prop in props) {
				bool value=e.Data.GetDataPresent(prop.Name);
				droppedWas.Add(prop.Name,value);
				//System.Diagnostics.Debug.WriteLine(String.Format("{0} = {1}",prop.Name,value),"Drag'n Drop");
			}
		}
	}
}
