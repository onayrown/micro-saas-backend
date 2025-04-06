import React, { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogTitle,
  IconButton,
  Slide
} from '@mui/material';
import { TransitionProps } from '@mui/material/transitions';
import CloseIcon from '@mui/icons-material/Close';
import ContentForm, { ContentFormMode } from './ContentForm';
import ContentService, { ContentPost, CreatePostRequest, UpdatePostRequest } from '../../services/ContentService';

const Transition = React.forwardRef(function Transition(
  props: TransitionProps & {
    children: React.ReactElement;
  },
  ref: React.Ref<unknown>,
) {
  return <Slide direction="up" ref={ref} {...props} />;
});

export interface ContentFormDialogProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
  mode: ContentFormMode;
  post?: ContentPost;
}

const ContentFormDialog: React.FC<ContentFormDialogProps> = ({
  open,
  onClose,
  onSuccess,
  mode,
  post
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSave = async (data: CreatePostRequest | UpdatePostRequest) => {
    setIsLoading(true);
    setError(null);
    
    try {
      if (mode === 'create') {
        await ContentService.createPost(data as CreatePostRequest);
      } else if (mode === 'edit' && 'id' in data) {
        const { id, ...updateData } = data;
        await ContentService.updatePost(id, updateData as UpdatePostRequest);
      }
      
      onSuccess();
      onClose();
    } catch (error: any) {
      setError(error.message || 'Ocorreu um erro ao salvar o conteúdo');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Dialog
      open={open}
      onClose={isLoading ? undefined : onClose}
      fullWidth
      maxWidth="md"
      TransitionComponent={Transition}
    >
      <DialogTitle>
        {mode === 'create' ? 'Criar Novo Conteúdo' : 'Editar Conteúdo'}
        {!isLoading && (
          <IconButton
            aria-label="close"
            onClick={onClose}
            sx={{
              position: 'absolute',
              right: 8,
              top: 8,
              color: (theme) => theme.palette.grey[500],
            }}
          >
            <CloseIcon />
          </IconButton>
        )}
      </DialogTitle>
      <DialogContent>
        <ContentForm
          mode={mode}
          post={post}
          onSave={handleSave}
          onCancel={onClose}
          isLoading={isLoading}
          error={error}
        />
      </DialogContent>
    </Dialog>
  );
};

export default ContentFormDialog; 