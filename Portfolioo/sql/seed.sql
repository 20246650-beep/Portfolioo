insert into portfolio.course (code, title, description) values
('DBMWEB1', 'Database Management and Web Systems 1', 'Databases and ASP.NET Core MVC.');

insert into portfolio.project (course_id, title, description, year, video_url)
select id, 'Library System', 'A library borrowing system with ASP.NET Core MVC.', 2026, null
from portfolio.course where code = 'DBMWEB1';

insert into portfolio.project_image (project_id, url)
select id, 'https://frelfzehjnuiqtrwinyl.supabase.co/storage/v1/object/public/portfolio-images/library1.png'
from portfolio.project where title = 'Library System';