﻿using Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warehouse
{
    static class Warehouse
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

           Uri warehouseUri = new Uri("http://localhost:4000/");

            WarehouseCommunicationHandler warehouseHandler = new WarehouseCommunicationHandler(warehouseUri);

            Application.Run(new WarehouseGUI(warehouseHandler));
        }
    }
}
