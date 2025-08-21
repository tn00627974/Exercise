using AutoMapper;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Profiles
{
    public class TodoProfile : Profile
    {
        public TodoProfile()
        {
            CreateMap<TodoList, TodoListSelectDto>();         
                //.ForMember(dest => dest.InsertEmployeeName, opt => opt.MapFrom(src => src.InsertEmployee.Name))
                //.ForMember(dest => dest.UpdateEmployeeName, opt => opt.MapFrom(src => src.UpdateEmployee.Name));                          
        }
    }
}
