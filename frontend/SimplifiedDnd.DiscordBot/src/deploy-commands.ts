import { REST, Routes } from 'discord.js';
import dotenv from 'dotenv';

import Bot from './abstractions/bot';
import Logger from './abstractions/logger';

dotenv.config();
const logger = new Logger();
const bot = new Bot(undefined!);

const rest = new REST().setToken(process.env.DISCORD_TOKEN ?? '');

bot.loadCommands().then(async () => {
  try {
    logger.logInfo(`Started refreshing ${bot.commandsHandler.size} application (/) commands.`);

    await rest.put(
      Routes.applicationGuildCommands(
        process.env.APPLICATION_ID ?? '',
        process.env.GUILD_ID ?? ''),
      { body: bot.commandsHandler.map(handler => handler.getCommand().toJSON()) },
    );

    logger.logInfo('Successfully reloaded application (/) commands.');
  } catch (error) {
    logger.logError(`Failed to refresh application commands: ${error}`);
  }
});
