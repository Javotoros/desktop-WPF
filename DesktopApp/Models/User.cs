using System;

namespace DesktopApp.Models
{
    public class User
    {
        public string Id { get; set; }            
        public string FirstName { get; set; } 
        public string LastName { get; set; }       
        public string Email { get; set; }          
        public string Password { get; set; }      
        public string DNI { get; set; }        
        public string PhoneNumber { get; set; }   
        public DateTime BirthDate { get; set; }   
        public string CityName { get; set; }       
        public string Gender { get; set; }         
        public string ImageRoute { get; set; }     
        public string Rol { get; set; }            
        public bool VipStatus { get; set; }        

        public string NombreCompleto => $"{FirstName} {LastName}";

        public string DisplayInfo => $"{DNI} - {FirstName} {LastName}";
    }
}

