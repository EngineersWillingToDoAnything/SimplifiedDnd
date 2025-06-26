import {
  ActionRowBuilder,
  CacheType,
  ChatInputCommandInteraction,
  MessageFlags,
  SlashCommandBuilder,
  SlashCommandOptionsOnlyBuilder,
  StringSelectMenuBuilder,
  StringSelectMenuInteraction,
  StringSelectMenuOptionBuilder,
} from 'discord.js';

import DndCommandHandler from '../abstractions/dnd/dnd-command-handler';

enum CommandOptions {
  CharacterName = 'name',
}

export default class NewCharacterCommandHandler extends DndCommandHandler {
  readonly command: SlashCommandOptionsOnlyBuilder;

  constructor() {
    super();
    this.command = new SlashCommandBuilder()
      .setName('new-character')
      .setDescription('Create a new character assigned to the user')
      .addStringOption(option =>
        option.setName(CommandOptions.CharacterName)
          .setDescription('The name of the character')
          .setRequired(true),
      );
  }

  getCommand(): SlashCommandBuilder {
    return this.command as SlashCommandBuilder;
  }

  protected async handleCommand(
    interaction: ChatInputCommandInteraction<CacheType>,
  ): Promise<void> {
    const characterName = interaction.options.getString(CommandOptions.CharacterName, true);

    const specieName = await this.getSpecie(interaction);
    const className = await this.getClass(interaction);
    if (!specieName || !className) {
      await interaction.editReply({
        content: 'Confirmation not received within 1 minute, cancelling',
        components: [],
      });
      return;
    }

    const characterId = await this.service?.createCharacter({
      name: characterName,
      playerId: interaction.user.id,
      specieName,
      className,
    });

    if (characterId === '') {
      await interaction.followUp({
        content: `Failed to create character \`${characterName}\`. Please try again later.`,
        flags: MessageFlags.Ephemeral,
      });
      return;
    }

    await interaction.followUp({
      content: `Character \`${characterName}\` has been created successfully!`,
    });
  }

  private getSpecieSelectMenu() {
    return new StringSelectMenuBuilder()
      .setCustomId('specie-menu')
      .setPlaceholder('Make a choice!')
      .addOptions(
        new StringSelectMenuOptionBuilder()
          .setLabel('Human')
          .setDescription('The common specie of the world.')
          .setValue('human'),
        // new StringSelectMenuOptionBuilder()
        //   .setLabel('Tiefling')
        //   .setDescription('Mixture of human and `something else`')
        //   .setValue('tiefling'),
        // new StringSelectMenuOptionBuilder()
        //   .setLabel('Halfling')
        //   .setDescription('Half tall as humans and not as stocky as dwarves.')
        //   .setValue('halfling'),
      );
  }

  private async getSpecie(
    interaction: ChatInputCommandInteraction<CacheType>,
  ): Promise<string | undefined> {
    const row = new ActionRowBuilder<StringSelectMenuBuilder>()
      .addComponents(this.getSpecieSelectMenu());

    const response = await interaction.reply({
      content: 'Please select your character\'s specie:',
      components: [row],
      withResponse: true,
    });

    try {
      const collected = await response.resource?.message?.awaitMessageComponent({
        filter: i => i.user.id === interaction.user.id,
        time: 60_000,
      });

      collected?.deferUpdate();
      return (collected as StringSelectMenuInteraction).values[0];
    } catch {
      return undefined;
    }
  }

  private getClassSelectMenu() {
    return new StringSelectMenuBuilder()
      .setCustomId('class-menu')
      .setPlaceholder('Make a choice!')
      .addOptions(
        new StringSelectMenuOptionBuilder()
          .setLabel('Artificer')
          .setValue('artificer'),
        new StringSelectMenuOptionBuilder()
          .setLabel('Barbarian')
          .setValue('barbarian'),
        new StringSelectMenuOptionBuilder()
          .setLabel('Bard')
          .setValue('bard'),
      );
  }

  private async getClass(
    interaction: ChatInputCommandInteraction<CacheType>,
  ): Promise<string | undefined> {
    const row = new ActionRowBuilder<StringSelectMenuBuilder>()
      .addComponents(this.getClassSelectMenu());

    const response = await interaction.editReply({
      components: [row],
    });

    try {
      const collected = await response.awaitMessageComponent({
        filter: i => i.user.id === interaction.user.id,
        time: 60_000,
      });

      collected?.deferUpdate();
      await interaction.deleteReply();
      return (collected as StringSelectMenuInteraction).values[0];
    } catch {
      return undefined;
    }
  }
};