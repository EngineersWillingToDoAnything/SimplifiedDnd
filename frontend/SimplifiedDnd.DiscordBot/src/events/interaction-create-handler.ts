import { Events, Interaction, CacheType, MessageFlags, ChatInputCommandInteraction } from 'discord.js';

import { BaseEventHandler } from '../abstractions/event-handler';
import Logger from '../abstractions/logger';
import Bot from '../abstractions/bot';

export default class InteractionCreateHandler extends BaseEventHandler<Events.InteractionCreate> {
  logger: Logger;

  constructor(private readonly bot: Bot) {
    super(false, Events.InteractionCreate);
    this.logger = new Logger();
  }

  protected handle = async (interaction: Interaction<CacheType>) => {
    if (interaction.isChatInputCommand()) {
      return this.handleChatInputCommand(interaction);
    }
  };

  private async handleChatInputCommand(interaction: ChatInputCommandInteraction<CacheType>) {
    const command = this.bot.commandsHandler.get(interaction.commandName);

    if (!command) {
      this.logger.logError(`No command matching ${interaction.commandName} was found.`);
      return;
    }

    try {
      await command.handle(interaction, this.bot);
    } catch (error) {
      this.logger.logError(`Error executing command ${interaction.commandName}: ${error}`);
      if (interaction.replied || interaction.deferred) {
        await interaction.followUp({
          content: 'There was an error while executing this command!',
          flags: MessageFlags.Ephemeral,
        });
      } else {
        await interaction.reply({
          content: 'There was an error while executing this command!',
          flags: MessageFlags.Ephemeral,
        });
      }
    }
  }
}