DROP TABLE IF EXISTS dbo.admin_department;
DROP TABLE IF EXISTS dbo.log_login;
DROP TABLE IF EXISTS dbo.[admin];
DROP TABLE IF EXISTS dbo.[rule];
DROP TABLE IF EXISTS dbo.department;
DROP TABLE IF EXISTS dbo.employee;
DROP TABLE IF EXISTS dbo.category;
DROP TABLE IF EXISTS dbo.capa;
DROP TABLE IF EXISTS dbo.related_work_unit;
DROP TABLE IF EXISTS dbo.root_cause;
DROP TABLE IF EXISTS dbo.correction_action;
DROP TABLE IF EXISTS dbo.verification;
DROP TABLE IF EXISTS dbo.email_log;
-- ==============================================================================
create table category (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	category_name nvarchar(255) not null UNIQUE,
	description nvarchar(max) not null,
	is_active int default 1,
	created_at datetime not null,
	updated_at datetime,
	updated_by nvarchar(255) not null
);
-- ==============================================================================
create table [rule] (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	rule_name nvarchar(255) not null UNIQUE,
);
-- ======================================================================================
create table department (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	department_name nvarchar(255) not null UNIQUE,
	is_active int default 1,
	created_at datetime not null,
	updated_at datetime,
	updated_by nvarchar(255) not null
);
-- ==============================================================================
create table employee (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	display_name nvarchar(255) not null UNIQUE,
	email nvarchar(255) not null UNIQUE,
	is_active int default 1,
	created_at datetime not null,
	updated_at datetime,
	updated_by nvarchar(255) not null
);
-- ==============================================================================

create table admin_department (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_department int not null,
	id_employee int not null,
	is_active int default 1,
	created_at datetime not null,
	updated_at datetime,
	updated_by nvarchar(255) not null
);

ALTER TABLE [dbo].[admin_department]  WITH CHECK ADD  CONSTRAINT [FK_admin_department_department] FOREIGN KEY([id_department])
REFERENCES [dbo].[department] ([id])
ON UPDATE CASCADE
ON DELETE NO ACTION
GO

ALTER TABLE [dbo].[admin_department]  WITH CHECK ADD  CONSTRAINT [FK_admin_department_employee] FOREIGN KEY([id_employee])
REFERENCES [dbo].[employee] ([id])
ON UPDATE CASCADE
ON DELETE NO ACTION
GO
-- ==============================================================================
create table admin (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_employee int not null,
	id_employee_spv int,
	description nvarchar(max) not null,
	id_rule int not null,
	is_active int default 1,
	created_at datetime not null,
	updated_at datetime,
	updated_by nvarchar(255) not null
);

ALTER TABLE [dbo].[admin]  WITH CHECK ADD  CONSTRAINT [FK_admin_employee] FOREIGN KEY([id_employee])
REFERENCES [dbo].[employee] ([id])
ON UPDATE CASCADE
ON DELETE NO ACTION
GO
-- ==============================================================================

create table log_login (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	id_employee int not null,
	[rule] [nvarchar](50) NOT NULL,
	[last_login] [datetime] NULL,
	[alamat_ip] [nvarchar](255) noT NULL,
	[browser] [nvarchar](255) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[deleted_at] [datetime] NULL,
);

-- ==============================================================================

create table capa (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	capa_no int not null UNIQUE,
	initiation_date datetime not null,
	source nvarchar(255) not null,
	problem nvarchar(max) not null,	
	initiator int not null,-- id_employee --initiator
	id_admin int not null, -- pic admin
	id_category int null, -- kategori capa (limbah, lingkungan , dsb)	
	is_proper nvarchar(150), -- usulan, berulang, layak
	status nvarchar(50),
	flag int, -- 1,2,3,4,5,6	
	is_active int default 1
);

-- ==============================================================================

create table related_work_unit (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	capa_no int,
	id_employee int not null, 
	[rule] nvarchar(100) not null, -- filler / follow-up / approval
	id_department int not null,
);

-- ==============================================================================

create table root_cause (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	capa_no int,
	root_cause nvarchar(max) not null,
	created_at datetime not null,
	updated_at datetime,
	updated_by int not null, -- id employee
);

-- ==============================================================================

create table correction_action (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	capa_no int,
	
	correction nvarchar(max) not null,
	
	pic nvarchar(max) not null,
	
	deadline datetime not null,
	realization datetime ,
	
	type_correction nvarchar(50) not null, -- correction atau corrective_preventive
	
	created_at datetime not null,
	updated_at datetime,
	updated_by int not null, -- id employee
);

-- ==============================================================================

create table verification (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	capa_no int,
	verification nvarchar(max) not null,
	created_at datetime not null,
	updated_at datetime,
	updated_by int not null, -- id employee
);

-- ==============================================================================

create table email_log (
	id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	capa_no int,
	email_to nvarchar(255) not null,
	email_from nvarchar(max) not null,
	contents nvarchar(max) not null,
	created_at datetime not null,
	updated_by int not null, -- id employee
);

===================================================================================
insert into employee (display_name, email, is_active, created_at, updated_at, updated_by)
values ('Teni Setiadi', 'teni.setiadi@medion.co.id', 1, getdate(), getdate(), 'teni.setiadi');

insert into employee (display_name, email, is_active, created_at, updated_at, updated_by)
values ('Joko', 'app_hris@medion.co.id', 1, getdate(), getdate(), 'teni.setiadi');

insert into employee (display_name, email, is_active, created_at, updated_at, updated_by)
values ('Amir', 'app_default@medion.co.id', 1, getdate(), getdate(), 'teni.setiadi');


insert into employee (display_name, email, is_active, created_at, updated_at, updated_by)
values ('Budi', 'teni.setiadi@yahoo.com', 1, getdate(), getdate(), 'teni.setiadi');

insert into employee (display_name, email, is_active, created_at, updated_at, updated_by)
values ('Santoso', 'teni.setiadi@gmail.com', 1, getdate(), getdate(), 'teni.setiadi');
---------------------------------------------------------------------------------------
insert into [rule] (rule_name) values ('ADMIN')
insert into [rule] (rule_name) values ('SUPERADMIN')
---------------------------------------------------------------------------------------
INSERT INTO admin (id_employee, id_employee_spv, description, id_rule, is_active, created_at, updated_at, updated_by)
values (4, 1, 'CAPA UMUM', 1,1,getdate(),getdate(),'teni.setiadi');

INSERT INTO admin (id_employee, id_employee_spv, description, id_rule, is_active, created_at, updated_at, updated_by)
values (5, 1, 'CAPA UMUM', 1,1,getdate(),getdate(),'teni.setiadi');

INSERT INTO admin (id_employee,  description, id_rule, is_active, created_at, updated_at, updated_by)
values (1, '-', 2,1,getdate(),getdate(),'teni.setiadi');
---------------------------------------------------------------------------------------
/*[related_work_unit]
rule = 'FILLER';
rule = 'FOLLOW_UP';
rule = 'APPROVAL';*/
===================================================================================
