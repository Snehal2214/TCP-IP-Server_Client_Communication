using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcpIp_ChatApp.Command;
using TcpIp_ChatApp.Model;
using TcpIp_ChatApp.Net;

namespace TcpIp_ChatApp.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        private Server _server;
        public string Username { get; set; }
        
        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            _server = new Server();
            _server.connectedEvent += UserConnected;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o=> !string.IsNullOrEmpty(Username));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };
            if (!Users.Any(x => x.UID == user.UID)) 
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}