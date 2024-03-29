﻿using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
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

        public CommentService(ApplicationDbContext db)
        {
            this.db = db;
        }


        public async Task<Comment> AddComment(AddCommentInputModel model)
        {
            //DateTime.ParseExact(DateTime.UtcNow.ToString(), "g", CultureInfo.InvariantCulture, DateTimeStyles.None), 
            var comment = new Comment
            {
                Description = model.Description,
                UserEmail = model.UserId
            };

            db.Comments.Add(comment);

            await db.SaveChangesAsync();

            return comment;
        }

        public List<CommentOutputDto> GetMovieComments(int id)
        {
            var d = this.db.MovieComments
                .Where(x => x.MovieId == id)
                .Join(this.db.Comments,
                a => a.CommentId,
                b => b.Id,
                (a, b) => b)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new CommentOutputDto 
                {
                    Id = x.Id,
                    Description = x.Description,
                    CreatedOn = x.CreatedOn,
                    UserEmail = x.UserEmail
                })
                .ToList();

            return d;
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }
    }
}
