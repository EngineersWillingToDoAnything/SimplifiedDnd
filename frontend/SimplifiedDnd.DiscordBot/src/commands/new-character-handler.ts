import { CacheType, ChatInputCommandInteraction, SlashCommandBuilder } from 'discord.js';

import CommandHandler from '../abstractions/command-handler';

export default class NewCharacterCommandHandler implements CommandHandler {
  command: SlashCommandBuilder;

  constructor() {
    this.command = new SlashCommandBuilder()
      .setName('new-character')
      .setDescription('Create a new character assigned to the user');
  }

  getCommand(): SlashCommandBuilder {
    return this.command;
  }

  async handle(interaction: ChatInputCommandInteraction<CacheType>): Promise<void> {
	  await interaction.reply('Pong!');
  }
};