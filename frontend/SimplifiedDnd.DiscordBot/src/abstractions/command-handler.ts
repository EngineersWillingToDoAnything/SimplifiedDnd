import { ChatInputCommandInteraction, CacheType, SlashCommandBuilder } from 'discord.js';

export default interface CommandHandler {
  handle(interaction: ChatInputCommandInteraction<CacheType>): Promise<void>;
  getCommand(): SlashCommandBuilder;
// eslint-disable-next-line semi
}