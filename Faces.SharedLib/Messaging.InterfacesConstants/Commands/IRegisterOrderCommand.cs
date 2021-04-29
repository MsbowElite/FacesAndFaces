using System;

namespace Messaging.InterfacesConstants.Commands
{
    public interface IRegisterOrderCommand
    {
        public Guid Id { get; set; }
        public string PictureUrl { get; set; }
        public string UserEmail { get; set; }
        public byte[] ImageData { get; set; }
    }
}
