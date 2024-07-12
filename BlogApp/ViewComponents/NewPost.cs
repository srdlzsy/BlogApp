using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class NewPost : ViewComponent
    {
        private readonly IPostRepository _postRepository;
        public NewPost(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _postRepository
                                .Posts
                                .OrderByDescending(p => p.PublishedOn)
                                .Take(5)
                                .ToListAsync()
            );
        }
    }
}