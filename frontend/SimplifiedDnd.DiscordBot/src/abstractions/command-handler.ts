import {
  ChatInputCommandInteraction,
  CacheType,
  SlashCommandBuilder,
} from 'discord.js';

import Bot from './bot';

export default interface CommandHandler {
  handle(
    interaction: ChatInputCommandInteraction<CacheType>,
    bot: Bot,
  ): Promise<void>;
  getCommand(): SlashCommandBuilder;
}
