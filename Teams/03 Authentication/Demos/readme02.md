# Demo 2: Connectors

<<<<<<< HEAD
## Use the new Profile command

1. In a channel conversation, "at" mention the bot and issue the command `profile`.

1. The bot will attempt to acquire a token for the current user from the Azure Bot Service. If the token is stale, missing, does not have the requested scopes or is otherwise not valid, the bot will reply with a sign-in card.

    ![Screenshot of bot with signin card](Images/Exercise2-01.png)

1. Once sign-in ins complete, the bot will access profile information for the current user and write a message.

    ![Screenshot of bot with profile information message](Images/Exercise2-02.png)
=======
## Connect to a channel

1. To add the connector, select the elipsis to the right of **General** channel in the team. Then select **Connectors**.

    ![Screenshot of Teams showing steps to add a connector](Images/Exercise2-03.png)

1. Connectors from uploaded Microsoft Teams app displayed at the bottom of this list. Scroll to the bottom and choose
**OfficeDev Talent Management**.

    ![Screenshot of Connector list highlighting the uploaded app](Images/Exercise2-04.png)

1. The Connector configuration page is displayed. Select **Save** to register the connector.

    ![Screenshot of connector configuration page](Images/Exercise2-05.png)
1. From the Connector list, select **OfficeDev Talent Management**. Select **Configure**. THe configuration page will display the webhook URL for posting to the channel.
>>>>>>> new module 3
