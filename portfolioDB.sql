USE Portfolio;

CREATE TABLE Skills
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	NameSkill NVARCHAR(100) NOT NULL UNIQUE,
	Level INT NOT NULL
);

INSERT INTO Skills(NameSkill, Level)
VALUES	('.NET', 80),
		('C#', 95),
		('React', 85),
		('Javascript', 75),
		('Typescript', 80),
		('SQL', 95),
		('React Native', 95),
		('Native', 70),
		('Docker', 80),
		('English', 70),
		('Soft-skill', 65);

CREATE TABLE Projects
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Title NVARCHAR(100) NOT NULL,
	Platform NVARCHAR(50) NOT NULL,
	Position NVARCHAR(50) NOT NULL,
	NumOfMember INT NOT NULL,
	Description NVARCHAR(MAX),
	ImageUrl NVARCHAR(256) NOT NULL
);

CREATE TABLE ProjectTags
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	ProjectId INT FOREIGN KEY REFERENCES Projects(Id),
	Tag NVARCHAR(100) NOT NULL,
);

INSERT INTO Projects (Title, Platform, Position, NumOfMember, Description, ImageUrl)
VALUES
	('Manage Student Application', 'Window Form', 'Back-end', 1, 
	 'A project running on Winform to manage student scores, built according to MVC model. Using web socket to transmit score changes in real-time, using big query to save student score information.\n\nlink Github: https://github.com/CoderSaiya/BTL_LTM',
	 '/manage_score.png'),

	('Goods Exchange', 'Web', 'Supporter for Front-end and Back-end', 7, 
	 'This project is the frontend for the Goods Exchange system between UTH students. The project is developed using ReactJS, Typescript, Tailwind, Redux, SockJs - Socket, Tippy, and UI library for frontend and Spring Boots Java for backend.\n\nlink Github BE: https://github.com/UTH-group/Goods_Exchange_Application\nlink Github FE: https://github.com/UTH-group/FE',
	 '/goods.png'),

	('Freelance Marketplace', 'Web', 'Sub-leader, Main FE + BE', 5, 
	 'Freelance Marketplace is an online platform that connects freelancers with clients who need services. This system makes the collaboration process safe, transparent and efficient, from posting projects, selecting freelancers, to payments and reviews.\n\nlink Github BE: https://github.com/CoderSaiya/BE_XDPMHDT\nlink Github FE: https://github.com/CoderSaiya/FE_XDPMHDT',
	 '/freelance.png'),

	('Movie Streaming', 'Web + Mobile App', 'Main FE + BE', 1, 
	 'MovieStream is a comprehensive microservices-based movie streaming platform built with .NET 8.0, designed to provide a scalable and high-performance movie streaming experience.\n\nlink Github: https://github.com/CoderSaiya/MovieStream',
	 '/movie_stream.png'),

	('Booking Movie Ticket', 'Mobile App', 'Main FE + BE', 1, 
	 'Booking Movie Ticket is a convenient and quick process of booking movie tickets, helping users choose their favorite movies, suitable showtimes, and ideal seats. Through applications or websites, users can look up information, book tickets online, and pay easily without having to queue at the theater, providing a modern and time-saving experience.\n\nlink Github: https://github.com/CoderSaiya/MovieTicket',
	 '/movie_ticket.png');

INSERT INTO ProjectTags(ProjectId, Tag)
VALUES	
		(1, 'Winform'),
		(1, '.NET Core'),
		(1, 'C#'),
		(1, 'Entity Framework'),
		(1, 'Web Socket'),
		(1, 'Google Big Query'),
		(2, 'Spring'),
		(2, 'Java'),
		(2, 'React'),
		(2, 'Redux Toolkit'),
		(2, 'Tailwinds CSS'),
		(3, '.NET Core'),
		(3, 'C#'),
		(3, 'SignalR'),
		(3, 'Entity Framework'),
		(3, 'React'),
		(3, 'RTK Query'),
		(3, 'shadcn/ui'),
		(3, 'Chart.js'),
		(3, 'Tailwinds CSS'),
		(4, '.NET Core'),
		(4, 'C#'),
		(4, 'Entity Framework'),
		(4, 'Microservice'),
		(4, 'Ocelot'),
		(4, 'RabbitMQ'),
		(4, 'React'),
		(4, 'React Native'),
		(5, 'React Native'),
		(5, 'Typescript'),
		(5, 'React Context'),
		(5, 'Ant Design'),
		(5, '.NET Core'),
		(5, 'C#'),
		(5, 'SignalR'),
		(5, 'Entity Framework');

SELECT * FROM Projects;
SELECT * FROM Skills;

DROP TABLE IF EXISTS ProjectTags;
DROP TABLE IF EXISTS Projects;
DROP TABLE IF EXISTS Skills