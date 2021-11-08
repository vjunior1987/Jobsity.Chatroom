# Jobsity.Chatroom.
Coding Challenge for Jobsity

Hello! This is my submission for the Jobsity coding challenge in Asp Net Core.

-------------------------------------------------------------------------------------------------

# Prerequisites to run the project
- You need Docker installed. You can do so at https://www.docker.com/get-started.
- You need RabbitMQ installed on your local machine. The configuration used was the one provided at https://www.rabbitmq.com/download.html. 
  execute the following line in your command prompt: docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management

# To test the solution
- First, execute the docker command above, and have the RabbitMQ container running in the background
- This project contains a decoupled bot that needs to be executed separately, in order to receive and process commands. Run it in a separate instance.
- Another instance might be required should you wish to test different users simultaneously


### Bonus requirements met
- v0.2 Treated different exceptions for Stock bot. It will return messages whether it didn't understand the command, the user asked for help or an error occurs.
- 
