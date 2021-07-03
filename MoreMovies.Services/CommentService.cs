﻿using Microsoft.AspNetCore.Identity;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext db;
        private readonly IdentityUser user;

        public CommentService(ApplicationDbContext db, IdentityUser user)
        {
            this.db = db;
            this.user = user;
        }


        public async Task<Comment> AddComment(AddCommentInputModel model, string email)
        {
            //DateTime.ParseExact(DateTime.UtcNow.ToString(), "g", CultureInfo.InvariantCulture, DateTimeStyles.None), 
            var comment = new Comment
            {
                Description = model.Description,
                UserFullName = email
            };

            db.Comments.Add(comment);

            await db.SaveChangesAsync();

            return comment;
        }

        public List<Comment> GetMovieComments(int id)
        {
            var d = this.db.MovieComments
                .Where(x => x.MovieId == id)
                .Join(this.db.Comments,
                a => a.CommentId,
                b => b.Id,
                (a, b) => b).ToList();

            return d;
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }
    }
}
