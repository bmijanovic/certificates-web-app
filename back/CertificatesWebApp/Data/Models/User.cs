﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [Table("users")]
    public class User : IBaseEntity
    {
        

        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("name")]
        public String Name { get; set; }

        [Column("surname")]
        public String Surname { get; set; }

        [Column("telephone")]
        public String? Telephone { get; set; }

        [Column("email")]
        public String Email { get; set; }

        [Column("activated")]
        public Boolean IsActivated { get; set; }
        public string Discriminator { get; private set; }

        public User()
        {
        }

        public User(Guid id, string name, string surname, string telephone, string email, bool isActivated)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Telephone = telephone;
            Email = email;
            IsActivated = isActivated;
        }
    }
}
