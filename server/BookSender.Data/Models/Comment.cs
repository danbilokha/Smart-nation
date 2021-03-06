﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookSender.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public string CommentBody { get; set; }

        public DateTime? CreatedOn { get; set; }
       
        public virtual User User { get; set; }

        public virtual Book Book { get; set; }
    }
}
