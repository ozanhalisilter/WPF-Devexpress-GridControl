using DevExpress.Xpf.Grid;
using System;
using System.Windows;
using DevExpress.Mvvm.UI;

namespace WPF_Devexpress_GridControl
{
    public interface IGridUpdateService
    {
        void BeginUpdate();
        void EndUpdate();
    }
    public class GridUpdateService : ServiceBase, IGridUpdateService
    {
        GridControl GridControl => AssociatedObject as GridControl;

        public void BeginUpdate()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (GridControl != null)
                {
                    GridControl.BeginDataUpdate();
                }
            }));
        }

        public void EndUpdate()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (GridControl != null)
                {
                    GridControl.EndDataUpdate();
                }
            }));
        }
    }
}
