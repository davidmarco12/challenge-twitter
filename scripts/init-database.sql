INSERT INTO "User"."User" ("UserName", "CreationDate") VALUES
('juan_developer', '2024-01-15 08:30:00'),
('maria_tech', '2024-01-16 09:15:00'),
('carlos_coder', '2024-01-17 10:45:00'),
('ana_designer', '2024-01-18 11:20:00'),
('luis_backend', '2024-01-19 14:30:00'),
('sofia_frontend', '2024-01-20 16:10:00'),
('diego_mobile', '2024-01-21 09:25:00'),
('laura_data', '2024-01-22 13:40:00'),
('pedro_devops', '2024-01-23 15:55:00'),
('carmen_ux', '2024-01-24 08:45:00'),
('roberto_qa', '2024-01-25 12:15:00'),
('elena_pm', '2024-01-26 17:30:00'),
('miguel_fullstack', '2024-01-27 10:20:00'),
('natalia_scrum', '2024-01-28 14:45:00'),
('javier_react', '2024-01-29 09:10:00'),
('patricia_node', '2024-01-30 11:35:00'),
('fernando_python', '2024-02-01 13:20:00'),
('gabriela_java', '2024-02-02 15:45:00'),
('alberto_csharp', '2024-02-03 08:55:00'),
('valeria_angular', '2024-02-04 16:25:00'),
('sergio_vue', '2024-02-05 12:40:00'),
('claudia_sql', '2024-02-06 09:30:00'),
('raul_mongodb', '2024-02-07 14:15:00'),
('monica_docker', '2024-02-08 10:50:00'),
('andres_k8s', '2024-02-09 13:05:00'),
('beatriz_aws', '2024-02-10 15:20:00'),
('oscar_azure', '2024-02-11 11:45:00'),
('lorena_gcp', '2024-02-12 08:30:00'),
('esteban_ml', '2024-02-13 16:40:00'),
('isabella_ai', '2024-02-14 12:25:00');

-- =====================================================
-- INSERTAR TWEETS (usando IDs auto-generados de usuarios)
-- =====================================================

INSERT INTO "Tweet"."Tweet" ("Content", "UserId", "CreationDate") VALUES
('¡Acabo de terminar mi primer proyecto en .NET 8! 🚀', 1, '2024-02-15 09:30:00'),
('Trabajando en un nuevo diseño de interfaz. Los usuarios van a amar esto! ✨', 4, '2024-02-15 10:15:00'),
('Docker + PostgreSQL = Combinación perfecta para desarrollo 🐳', 24, '2024-02-15 11:45:00'),
('Implementando autenticación JWT en mi API REST. Seguridad ante todo! 🔒', 5, '2024-02-15 14:20:00'),
('React Hooks hacen que el desarrollo frontend sea mucho más limpio 💙', 15, '2024-02-15 16:30:00'),
('Debugging a las 2 AM... el café es mi mejor amigo ☕', 3, '2024-02-16 02:15:00'),
('Machine Learning está cambiando el mundo. ¡Qué momento para estar vivo! 🤖', 29, '2024-02-16 08:45:00'),
('Code review completado. El trabajo en equipo hace la diferencia 👥', 11, '2024-02-16 13:20:00'),
('Desplegando a producción con CI/CD. La automatización es hermosa 🎯', 9, '2024-02-16 17:40:00'),
('SQL queries optimizadas = aplicación más rápida. Performance matters! ⚡', 22, '2024-02-17 09:10:00'),
('Nuevo sprint planificado. Este va a ser épico! 📋', 14, '2024-02-17 10:30:00'),
('Angular 17 tiene características increíbles. Frontend nunca fue tan poderoso 🅰️', 20, '2024-02-17 12:45:00'),
('Refactorizando código legacy. Es terapéutico y frustrante a la vez 😅', 13, '2024-02-17 15:20:00'),
('Python + Data Science = Magic ✨📊', 17, '2024-02-17 18:30:00'),
('UX research completed! Users insights are pure gold 💎', 10, '2024-02-18 09:00:00'),
('Node.js performance optimization. From 2s to 200ms response time! 🏃‍♂️💨', 16, '2024-02-18 11:15:00'),
('Kubernetes cluster deployed successfully. Scaling made easy! ☸️', 25, '2024-02-18 14:45:00'),
('Java Spring Boot + Microservices architecture. Enterprise ready! ☕', 18, '2024-02-18 16:20:00'),
('Mobile app published to App Store. Dreams come true! 📱🍎', 7, '2024-02-19 08:30:00'),
('Data pipeline processing 1M records per minute. Big Data feels good! 📈', 8, '2024-02-19 12:10:00'),
('Vue.js composition API is a game changer. Reactive programming FTW! 💚', 21, '2024-02-19 15:40:00'),
('MongoDB aggregation pipeline solved my complex query. NoSQL power! 🍃', 23, '2024-02-19 17:55:00'),
('AWS Lambda function deployed. Serverless architecture rocks! ☁️⚡', 26, '2024-02-20 09:25:00'),
('C# LINQ queries make data manipulation so elegant 💜', 19, '2024-02-20 13:30:00'),
('Azure DevOps pipeline configured. Continuous everything! 🔄', 27, '2024-02-20 16:15:00'),
('Google Cloud Platform monitoring setup complete. Observability matters! 📊', 28, '2024-02-21 10:20:00'),
('AI model accuracy improved to 95%. Machine learning magic! 🎯🤖', 30, '2024-02-21 14:45:00'),
('Full-stack development is like being a digital architect 🏗️', 13, '2024-02-21 18:30:00'),
('Code documentation updated. Future me will thank present me 📚', 12, '2024-02-22 11:10:00'),
('Open source contribution merged! Giving back to the community 🌟', 2, '2024-02-22 16:25:00');


INSERT INTO "User"."UserFollow" ("FollowerId", "FollowingId", "CreationDate") VALUES
-- juan_developer sigue a varios desarrolladores
(1, 2, '2024-02-01 10:30:00'),   -- juan_developer -> maria_tech
(1, 5, '2024-02-01 11:15:00'),   -- juan_developer -> luis_backend
(1, 15, '2024-02-01 12:45:00'),  -- juan_developer -> javier_react

-- maria_tech construye su red
(2, 1, '2024-02-02 09:20:00'),   -- maria_tech -> juan_developer
(2, 6, '2024-02-02 14:30:00'),   -- maria_tech -> sofia_frontend
(2, 29, '2024-02-02 16:10:00'),  -- maria_tech -> esteban_ml

-- carlos_coder sigue a mentores
(3, 13, '2024-02-03 08:45:00'),  -- carlos_coder -> miguel_fullstack
(3, 17, '2024-02-03 13:20:00'),  -- carlos_coder -> fernando_python
(3, 19, '2024-02-03 15:40:00'),  -- carlos_coder -> alberto_csharp

-- ana_designer conecta con UX/UI
(4, 10, '2024-02-04 11:30:00'), -- ana_designer -> carmen_ux
(4, 12, '2024-02-04 16:45:00'), -- ana_designer -> elena_pm
(4, 6, '2024-02-04 18:20:00'),  -- ana_designer -> sofia_frontend

-- luis_backend forma equipo
(5, 1, '2024-02-05 09:15:00'),  -- luis_backend -> juan_developer
(5, 16, '2024-02-05 12:30:00'), -- luis_backend -> patricia_node
(5, 22, '2024-02-05 17:45:00'), -- luis_backend -> claudia_sql

-- Conexiones cruzadas populares
(15, 20, '2024-02-06 10:20:00'), -- javier_react -> valeria_angular
(20, 21, '2024-02-06 14:15:00'), -- valeria_angular -> sergio_vue
(9, 24, '2024-02-06 16:30:00'),  -- pedro_devops -> monica_docker
(24, 25, '2024-02-07 09:45:00'), -- monica_docker -> andres_k8s
(25, 26, '2024-02-07 13:20:00'), -- andres_k8s -> beatriz_aws

-- Cloud specialists network
(26, 27, '2024-02-08 11:10:00'), -- beatriz_aws -> oscar_azure
(27, 28, '2024-02-08 15:25:00'), -- oscar_azure -> lorena_gcp
(28, 29, '2024-02-08 17:40:00'), -- lorena_gcp -> esteban_ml

-- AI/ML enthusiasts
(29, 30, '2024-02-09 10:35:00'), -- esteban_ml -> isabella_ai
(30, 8, '2024-02-09 14:50:00'),  -- isabella_ai -> laura_data
(8, 17, '2024-02-09 16:15:00'),  -- laura_data -> fernando_python

-- QA and PM connections
(11, 14, '2024-02-10 09:25:00'), -- roberto_qa -> natalia_scrum
(14, 12, '2024-02-10 13:40:00'), -- natalia_scrum -> elena_pm
(12, 11, '2024-02-10 17:20:00'), -- elena_pm -> roberto_qa

-- Full circle - popular follows
(13, 1, '2024-02-11 12:30:00'); -- miguel_fullstack -> juan_developer
