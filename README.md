# Jobsity.Chatroom.
Coding Challenge for Jobsity

Hello! This is my submission for the Jobsity coding challenge in Asp Net Core.

-------------------------------------------------------------------------------------------------

# Prerequisites to run the project
- Have a version of SQL Server installed. Alternatively, you can  alter the connectionString in the file 'appsettings.json' if you wish to use another Database.
- You need Docker installed. You can do so at https://www.docker.com/get-started.
- You need RabbitMQ installed on your local machine. The configuration used was the one provided at https://www.rabbitmq.com/download.html. 
  execute the following line in your command prompt: docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management

# To test the solution
- First, execute the docker command above, and have the RabbitMQ container running in the background
- Install the Stock Bot <a href="https://drive.google.com/file/d/1DYbNqeHhC78SeihF_5iHgDYNNP2hZBvR/view?usp=sharing ">x64</a> or <a href="https://drive.google.com/file/d/14juKqatI69KpuhBbIFQx5g9wbzxcd0x2/view?usp=sharing">x86</a> and execute it before running the web application. Alternatively, you can execute the Console project in a separate instance.
- Once that is set, execute the web application Jobsity.Chatroom. The web application might prompt you to migrate the database definitions upon user registration the first time.
- Run another instance of the web application should you wish to test different users simultaneously


### Bonus requirements met
- v0.1 Microsoft Identity for authorization
- v0.2 Treated different exceptions for Stock bot. It will return messages whether it didn't understand the command, the user asked for help or an error occurs.
- v0.3 Multiple chatrooms
- v0.4 Added installer for the Stock Bot using WIX
