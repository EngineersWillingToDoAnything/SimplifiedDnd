import {
  CacheType,
  ChatInputCommandInteraction,
  MessageFlags,
  SlashCommandBuilder,
} from 'discord.js';

import CommandHandler from '../command-handler';
import Bot from '../bot';
import DndService from './dnd-service';

export default abstract class DndCommandHandler implements CommandHandler {
  service: DndService | undefined = undefined;

  addDndService(service: DndService): void {
    this.service = service;
  }

  async handle(
    interaction: ChatInputCommandInteraction<CacheType>,
    bot: Bot,
  ): Promise<void> {
    if (!this.service) {
      await interaction.reply({
        content:  'DND service is not initialized. Please try again later.',
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    return this.handleCommand(interaction, bot);
  }

  abstract getCommand(): SlashCommandBuilder;

  protected abstract handleCommand(
    interaction: ChatInputCommandInteraction<CacheType>,
    bot: Bot,
  ): Promise<void>;
}