create schema if not exists portfolio;

create table portfolio.course (
    id          integer generated always as identity,
    code        varchar(20)  not null,
    title       varchar(100) not null,
    description text,
    constraint pk_course primary key (id),
    constraint uq_course_code unique (code)
);

create table portfolio.project (
    id          integer generated always as identity,
    course_id   integer      not null,
    title       varchar(100) not null,
    description text,
    year        integer      not null,
    video_url   varchar(300),
    constraint pk_project primary key (id),
    constraint fk_project_course foreign key (course_id)
        references portfolio.course (id) on delete restrict,
    constraint ck_project_year check (year between 2000 and 2100)
);

create table portfolio.project_image (
    id          integer generated always as identity,
    project_id  integer       not null,
    url         varchar(500)  not null,
    constraint pk_project_image primary key (id),
    constraint fk_image_project foreign key (project_id)
        references portfolio.project (id) on delete cascade,
    constraint uq_image_project_url unique (project_id, url)
);